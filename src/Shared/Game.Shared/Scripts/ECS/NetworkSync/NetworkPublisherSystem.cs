using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Arch.Core;
using Arch.System;
using Game.Shared.Scripts.Network;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.ECS.NetworkSync;

public enum SendCommandType { SendTo, SendToAll, SendToAllExcept }

// 1) Pool de buffers nativo (alloc/free em memória não gerenciada)
public static unsafe class NativeBufferPool
{
    private const int MaxPoolSize = 4096;
    private static readonly ConcurrentStack<(IntPtr Ptr, int Size)> Stack = new();
    private static int _currentPoolSize = 0;

    public static IntPtr Rent(int size)
    {
        while (Stack.TryPop(out var item))
        {
            if (item.Size >= size)
            {
                Interlocked.Decrement(ref _currentPoolSize);
                return item.Ptr;
            }
            else
            {
                Marshal.FreeHGlobal(item.Ptr);
                Interlocked.Decrement(ref _currentPoolSize);
            }
        }

        try
        {
            return Marshal.AllocHGlobal(size);
        }
        catch (OutOfMemoryException)
        {
            throw new InvalidOperationException($"Falha ao alocar {size} bytes de memória nativa. Verifique o tamanho solicitado ou a disponibilidade de memória.");
        }
    }

    public static void Return(IntPtr ptr, int size)
    {
        if (Interlocked.Increment(ref _currentPoolSize) > MaxPoolSize)
        {
            Interlocked.Decrement(ref _currentPoolSize);
            Marshal.FreeHGlobal(ptr);
        }
        else
        {
            Stack.Push((ptr, size));
        }
    }

    public static void Clear()
    {
        while (Stack.TryPop(out var item))
        {
            Marshal.FreeHGlobal(item.Ptr);
            Interlocked.Decrement(ref _currentPoolSize);
        }
    }
}

// 2) Comando de envio em memória nativa
public unsafe struct SendCommand
{
    public SendCommandType Type;
    public int PeerId;
    public DeliveryMethod Method;
    public byte* Buffer;
    public int Length;

    public void FreeBuffer()
    {
        if (Buffer != null)
        {
            NativeBufferPool.Return((IntPtr)Buffer, Length);
            Buffer = null;
            Length = 0;
        }
    }
}

// 3) Ring buffer SPSC para enfileiramento sem locks
public unsafe class SendCommandRingBuffer
{
    private readonly SendCommand* _buffer;
    private readonly int _capacity;
    private int _head;
    private int _tail;

    public SendCommandRingBuffer(int capacity)
    {
        if (capacity <= 1) throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be > 1");
        _capacity = capacity;
        var size = sizeof(SendCommand) * capacity;
        try
        {
            _buffer = (SendCommand*)Marshal.AllocHGlobal(size);
        }
        catch (OutOfMemoryException)
        {
            throw new InvalidOperationException($"Falha ao alocar buffer ring de {capacity} elementos. Tamanho total: {size} bytes.");
        }
        _head = _tail = 0;
    }

    public int Count => (_tail - _head + _capacity) % _capacity;
    public bool IsEmpty => _head == _tail;
    public bool IsFull => ((_tail + 1) % _capacity) == _head;

    public bool TryEnqueue(in SendCommand cmd)
    {
        int currentTail = _tail;
        int nextTail = (currentTail + 1) % _capacity;
        // Full check
        if (nextTail == Volatile.Read(ref _head))
        {
            Console.WriteLine("[WARN] SendCommandRingBuffer está cheio. Comando descartado.");
            return false;
        }
        _buffer[currentTail] = cmd;
        Volatile.Write(ref _tail, nextTail);
        return true;
    }

    public bool TryDequeue(out SendCommand cmd)
    {
        int currentHead = _head;
        // Empty check
        if (currentHead == Volatile.Read(ref _tail))
        {
            cmd = default;
            return false;
        }
        cmd = _buffer[currentHead];
        Volatile.Write(ref _head, (currentHead + 1) % _capacity);
        return true;
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal((IntPtr)_buffer);
    }
}

// 4) Sistema de publicação com buffer ring
public unsafe sealed class NetworkPublisherSystem : BaseSystem<World, float>
{
    private NetworkManager _net;
    private NetDataWriter _writer;
    private readonly SendCommandRingBuffer _ring;
    private readonly int _maxMessagesPerFrame;

    public NetworkPublisherSystem(World world, NetworkManager net,
        int ringCapacity = 1024, int maxMessagesPerFrame = 256)
        : base(world)
    {
        _net = net ?? throw new ArgumentNullException(nameof(net));
        _writer = new NetDataWriter();
        _ring = new SendCommandRingBuffer(ringCapacity);
        _maxMessagesPerFrame = maxMessagesPerFrame;
    }

    public void SendTo<T>(int peerId, T message, DeliveryMethod method)
        where T : unmanaged, INetSerializable
    {
        _writer.Reset();
        _net.Processor.WriteNetSerializable(_writer, ref message);

        int len = _writer.Length;
        var ptr = NativeBufferPool.Rent(len);
        fixed (byte* src = _writer.Data)
            Buffer.MemoryCopy(src, (byte*)ptr, len, len);

        var cmd = new SendCommand {
            Type = SendCommandType.SendTo,
            PeerId = peerId,
            Method = method,
            Buffer = (byte*)ptr,
            Length = len
        };
        if (!_ring.TryEnqueue(cmd))
        {
            Console.WriteLine($"[WARN] Falha ao enfileirar comando SendTo para peer {peerId}");
            cmd.FreeBuffer();
        }
    }

    public void SendToAll<T>(T message, DeliveryMethod method)
        where T : unmanaged, INetSerializable
    {
        _writer.Reset();
        _net.Processor.WriteNetSerializable(_writer, ref message);
        int len = _writer.Length;
        var ptr = NativeBufferPool.Rent(len);
        fixed (byte* src = _writer.Data)
            Buffer.MemoryCopy(src, (byte*)ptr, len, len);

        var cmd = new SendCommand {
            Type = SendCommandType.SendToAll,
            PeerId = -1,
            Method = method,
            Buffer = (byte*)ptr,
            Length = len
        };
        if (!_ring.TryEnqueue(cmd))
        {
            Console.WriteLine("[WARN] Falha ao enfileirar comando SendToAll");
            cmd.FreeBuffer();
        }
    }

    public void SendToAllExcept<T>(int exceptPeerId, T message, DeliveryMethod method)
        where T : unmanaged, INetSerializable
    {
        _writer.Reset();
        _net.Processor.WriteNetSerializable(_writer, ref message);
        int len = _writer.Length;
        var ptr = NativeBufferPool.Rent(len);
        fixed (byte* src = _writer.Data)
            Buffer.MemoryCopy(src, (byte*)ptr, len, len);

        var cmd = new SendCommand {
            Type = SendCommandType.SendToAllExcept,
            PeerId = exceptPeerId,
            Method = method,
            Buffer = (byte*)ptr,
            Length = len
        };
        if (!_ring.TryEnqueue(cmd))
        {
            Console.WriteLine($"[WARN] Falha ao enfileirar comando SendToAllExcept para exceto peer {exceptPeerId}");
            cmd.FreeBuffer();
        }
    }

    public override void Update(in float delta)
    {
        int processed = 0;
        while (processed < _maxMessagesPerFrame && _ring.TryDequeue(out var cmd))
        {
            var span = new ReadOnlySpan<byte>(cmd.Buffer, cmd.Length);
            switch (cmd.Type)
            {
                case SendCommandType.SendTo:
                    if (_net.NetManager.TryGetPeerById(cmd.PeerId, out var peer))
                        peer.Send(span, 0, cmd.Method);
                    break;
                case SendCommandType.SendToAll:
                    _net.NetManager.SendToAll(span, cmd.Method);
                    break;
                case SendCommandType.SendToAllExcept:
                    if (_net.NetManager.TryGetPeerById(cmd.PeerId, out var peer1))
                        _net.NetManager.SendToAll(span, cmd.Method, peer1);
                    break;
            }
            cmd.FreeBuffer();
            processed++;
        }
    }

    public override void Dispose()
    {
        // libera itens pendentes
        while (_ring.TryDequeue(out var cmd))
            cmd.FreeBuffer();
        _ring.Dispose();
        NativeBufferPool.Clear();
        // limpa writer e net
        _writer.Reset();
        _writer = null;
        _net = null;
        base.Dispose();
    }
}
