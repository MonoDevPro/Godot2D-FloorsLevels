using Arch.Core;
using Arch.LowLevel;
using Arch.System;
using Game.Shared.Scripts.Network;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.ECS.NetworkSync;

public sealed class LambdaNetReceiveSystem<T> : BaseSystem<World, float>
    where T : unmanaged, INetSerializable
{
    private UnsafeQueue<(T Message, int PeerId)> _buffer;
    private readonly Action<World, T, int> _handler;
    private readonly int _maxMessagesPerFrame;
    private readonly IDisposable _subscription;

    public LambdaNetReceiveSystem(
        World world,
        NetworkManager net,
        Action<World, T, int> handler,
        int maxMessagesPerFrame = 128,
        int initialBufferCapacity = 64)
        : base(world)
    {
        _handler = handler;
        _maxMessagesPerFrame = maxMessagesPerFrame;
        _buffer = new UnsafeQueue<(T, int)>(initialBufferCapacity);

        _subscription = net.Receiver.SubscribeSerializableMessage<T, int>(OnReceive);
    }

    private void OnReceive(T msg, int peerId)
    {
        _buffer.Enqueue((msg, peerId));
    }

    public override void Update(in float delta)
    {
        int count = Math.Min(_buffer.Count, _maxMessagesPerFrame);
        for (int i = 0; i < count; i++)
        {
            (T msg, int peer) = ref _buffer.Dequeue();
            _handler(World, msg, peer);
        }
    }

    public override void Dispose()
    {
        _subscription.Dispose();
        _buffer.Dispose();
    }
}
