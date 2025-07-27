using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Game.Shared.Scripts.ECS.Components;
using Game.Shared.Scripts.Network.Data;
using GameServer.Scripts.Root.Network;
using Godot;

namespace GameServer.Scripts.Root.ECS.Systems.Process.Out;

public partial class ServerOutputProcessSystem(World world, ServerNetwork network) : BaseSystem<World, float>(world)
{
    private ServerSender Sender => network.Sender;
    
    // 1) Tick control
    private const float TickInterval = 0.1f; // 10ms por segundo
    private const int MaxTicksPerFrame = 5; // Limita a 5 ticks por frame, para evitar sobrecarga
    private float _accum = 0f;

    // 2) Cache de último estado enviado
    private readonly Dictionary<int, (Vector2 pos, Vector2 vel)> _lastSent 
        = new();

    public override void Update(in float delta)
    {
        // 1) Acumula o tempo
        _accum += delta;
        if (_accum < TickInterval)
            return;
        
        // 2) Processa múltiplos ticks se necessário
        int ticks = 0;
        while (_accum >= TickInterval && ticks < MaxTicksPerFrame)
        {
            base.Update(TickInterval);
            _accum -= TickInterval;
            ticks++;
        }
    }

    // 3) Em vez de enviar um por um, montamos uma lista e enviamos em batch
    private readonly  List<StateMessage> _toSend = new(128);
    [Query]
    [All<NetworkedTag, PositionComponent, VelocityComponent>]
    private void SyncStateToClient(
        [Data] in float delta,
        in NetworkedTag tag,
        in PositionComponent position,
        in VelocityComponent velocity)
    {
        var id = tag.Id;
        var pos = position.Position;
        var vel = velocity.Velocity;

        // Checa se mudou suficiente para enviar
        if (_lastSent.TryGetValue(id, out var last))
        {
            const float POS_EPS = 0.01f;
            const float VEL_EPS = 0.01f;
            if ((last.pos - pos).LengthSquared() < POS_EPS * POS_EPS &&
                (last.vel - vel).LengthSquared() < VEL_EPS * VEL_EPS)
            {
                // sem mudança relevante, pula
                return;
            }
        }

        // Prepara mensagem
        _toSend.Add(new StateMessage {
            NetworkedTag = tag,
            NewPosition  = pos,
            NewVelocity  = vel
        });

        // Atualiza cache
        _lastSent[id] = (pos, vel);
    }

    // Depois que o Query acabar, manda o pacote
    public override void AfterUpdate(in float delta)
    {
        if (_toSend.Count <= 0) 
            return;
        
        // Broadcast para todos, ou adapte para cliente específico
        Sender.BroadcastStateMessage(_toSend.ToArray());
        _toSend.Clear();
    }
}