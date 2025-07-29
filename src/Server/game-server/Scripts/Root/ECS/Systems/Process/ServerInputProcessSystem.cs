using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Game.Shared.Scripts.ECS.Components;

namespace GameServer.Scripts.Root.ECS.Systems.Process;

public partial class ServerInputProcessSystem(World world) : BaseSystem<World, float>(world)
{
    // Aplica o Input na velocidade do Jogador
    [Query]
    [All<InputCommandComponent, VelocityComponent, SpeedComponent>]
    private void ApplyInputs(in InputCommandComponent cmd, ref VelocityComponent vel, in SpeedComponent sp)
    {
        vel.Value = cmd.Value.Normalized() * sp.Value; // Aplica o Input na velocidade
    }
}
