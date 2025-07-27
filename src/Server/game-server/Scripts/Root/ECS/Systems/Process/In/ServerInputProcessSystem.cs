using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Game.Shared.Scripts.ECS.Components;
using Game.Shared.Scripts.Network.Data;
using GameServer.Scripts.Root.Network;
using LiteNetLib;

namespace GameServer.Scripts.Root.ECS.Systems.Process.In;

public partial class ServerInputProcessSystem(World world) : BaseSystem<World, float>(world)
{
    // Aplica o Input na velocidade do Jogador
    [Query]
    [All<InputCommandComponent, VelocityComponent, SpeedComponent>]
    private void ApplyInputs(in InputCommandComponent cmd, ref VelocityComponent vel, in SpeedComponent sp)
    {
        vel.Velocity = cmd.Input.Normalized() * sp.Speed; // Aplica o Input na velocidade
    }
}
