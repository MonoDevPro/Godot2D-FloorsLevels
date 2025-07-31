using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Game.Shared.Scripts.ECS.Components;

namespace Game.Shared.Scripts.ECS.System.Physics;

/// <summary>
/// Sincroniza o estado final do nó CharacterBody2D (após MoveAndSlide)
/// de volta para os componentes do ECS (PositionComponent e VelocityComponent).
/// Este sistema atua como a ponte "de entrada" do motor de física para o ECS,
/// garantindo que o estado do ECS reflita o resultado da simulação.
/// </summary>
// TODO: [UpdateInGroup(typeof(SimulationSystemGroup))]
// GARANTE que rode DEPOIS que o passo de física foi executado pelo InputPhysicsSystem.
// [UpdateAfter(typeof(InputPhysicsSystem))]
public partial class OutputPhysicsSystem(World world) : BaseSystem<World, float>(world)
{
    [Query]
    [All<SceneBodyRefComponent>]
    private void SyncPhysicsStateToECS(in SceneBodyRefComponent body, ref PositionComponent pos, ref VelocityComponent vel)
    {
        // Verificação de segurança.
        if (body.Value == null || !Godot.GodotObject.IsInstanceValid(body.Value))
        {
            return;
        }

        // 1) Lê a posição final do corpo físico e a atualiza no ECS.
        // GlobalPosition é a posição final autoritativa após o movimento.
        pos.Value = body.Value.GlobalPosition;

        // 2) Lê a velocidade final e a atualiza no ECS.
        // A velocidade pode ter mudado devido a colisões (ex: bater em uma parede zera a velocidade
        // naquele eixo). É crucial capturar este resultado para que outros sistemas (como animação)
        // usem a velocidade real do personagem.
        vel.Value = body.Value.Velocity;
    }
}
