using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Game.Shared.Scripts.ECS.Components;

namespace Game.Shared.Scripts.ECS.System.Process;
/// <summary>
/// Sistema de movimento simplificado, projetado para rodar em um servidor headless.
/// Ele aplica a velocidade diretamente à posição, sem usar o motor de física do Godot.
/// Este sistema NÃO deve ser usado no cliente.
/// </summary>
// TODO: [UpdateInGroup(typeof(SimulationSystemGroup))]
// TODO: [UpdateAfter(typeof(InputApplySystem))] // Garante que a velocidade já tenha sido calculada
public partial class MovementSystem(World world) : BaseSystem<World, float>(world)
{
    [Query]
    [All<PositionComponent>]
    private void Move([Data] in float delta, ref PositionComponent pos, in VelocityComponent vel)
    {
        // Física simples: Posição = Posição + Velocidade * DeltaTime
        pos.Value += vel.Value * delta;
    }
}
