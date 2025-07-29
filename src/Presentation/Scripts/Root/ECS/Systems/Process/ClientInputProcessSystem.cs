using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Game.Shared.Scripts.ECS.Components;
using Godot;
using GodotFloorLevels.Scripts.Config;
using GodotFloorLevels.Scripts.Root.Network;

namespace GodotFloorLevels.Scripts.Root.ECS.Systems.Process;

public partial class ClientInputProcessSystem(World world, ClientNetwork network) : BaseSystem<World, float>(world)
{
    private Vector2 _lastSentInput = Vector2.Zero;
    private float _sendAccumulator = 0f;
    private const float SendInterval = 0.05f; // 20 vezes por segundo

    [Query] // Client-side movement system: applies Godot Input to InputCommand
    [All<InputCommandComponent>]
    private void UpdateInputCommand([Data]in float delta, ref InputCommandComponent cmd)
    {
        // Get input from Godot's InputMap
        cmd.Value = GodotInputMap.GetMovementInput();
        
        if (cmd.Value.Length() > 1f) // Se o input for muito alto, normaliza
            cmd.Value = cmd.Value.Normalized();
        else if (cmd.Value.Length() < 0.1f) // Se o input for muito baixo, zera
            cmd.Value = Vector2.Zero;
        
        // Send
        _sendAccumulator += delta;
        if (_sendAccumulator >= SendInterval && cmd.Value.DistanceTo(_lastSentInput) > 0.01f)
        {
            _sender.SendInputVector(cmd.Value);
            _lastSentInput = cmd.Value;
            _sendAccumulator = 0f;
        }
    }
    
    [Query] // Client-side movement system: applies InputCommand to velocity
    [All<InputCommandComponent, VelocityComponent, SpeedComponent>]
    private void UpdateLocalVelocity(ref InputCommandComponent cmd, ref VelocityComponent vel, in SpeedComponent sp)
    {
        vel.Value = cmd.Value.Normalized() * sp.Value; // Aplica o Input na velocidade
    }
}

