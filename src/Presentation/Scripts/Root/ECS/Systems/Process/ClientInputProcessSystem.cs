using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Game.Shared.Scripts.ECS.Components;
using Game.Shared.Scripts.ECS.NetworkSync;
using Game.Shared.Scripts.Network.Data.Input;
using Godot;
using GodotFloorLevels.Scripts.Config;
using GodotFloorLevels.Scripts.Root.Network;
using LiteNetLib;

namespace GodotFloorLevels.Scripts.Root.ECS.Systems.Process;

public partial class ClientInputProcessSystem(
    World world, NetworkPublisherSystem publisher) 
    : BaseSystem<World, float>(world)
{
    private Vector2 _lastSentInput = Vector2.Zero;
    private float _sendAccumulator = 0f;
    private const float SendInterval = 0.05f; // 20 vezes por segundo

    [Query] // Client-side movement system: applies Godot Input to InputCommand
    [All<PlayerControllerTag>]
    private void UpdateInputCommand([Data]in float delta, ref InputCommandComponent cmd)
    {
        // Get input from Godot's InputMap
        cmd.Value = GodotInputMap.GetMovementInput();
        
        if (cmd.Value.Length() > 1f) // Se o input for muito alto, normaliza
            cmd.Value = cmd.Value.Normalized();
        else if (cmd.Value.Length() < 0.1f) // Se o input for muito baixo, zera
            cmd.Value = Vector2.Zero;
    }
    
    [Query] // Client-side movement system: Send InputCommand to server
    [All<InputCommandComponent, VelocityComponent, SpeedComponent>]
    private void SendInput([Data]in float delta, ref InputCommandComponent cmd)
    {
        _sendAccumulator += delta;
        if (_sendAccumulator >= SendInterval && cmd.Value.DistanceTo(_lastSentInput) > 0.01f)
        {
            publisher.SendTo(0,new InputRequest { Value = cmd.Value }, DeliveryMethod.ReliableOrdered);
            _lastSentInput = cmd.Value;
            _sendAccumulator = 0f;
        }
    }
    
    [Query] // Client-side movement system: applies InputCommand to velocity
    [All<InputCommandComponent, VelocityComponent, SpeedComponent>]
    private void ApplyInput(ref InputCommandComponent cmd, ref VelocityComponent vel, in SpeedComponent sp)
    {
        vel.Value = cmd.Value.Normalized() * sp.Value; // Aplica o Input na velocidade
    }
}

