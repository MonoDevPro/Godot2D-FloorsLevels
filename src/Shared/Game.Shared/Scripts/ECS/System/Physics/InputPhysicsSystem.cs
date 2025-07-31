using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Game.Shared.Scripts.ECS.Components;

namespace Game.Shared.Scripts.ECS.System.Physics;

/// <summary>
/// Aplica o estado de velocidade do ECS no nó CharacterBody2D do Godot
/// e executa o principal passo de simulação de física chamando MoveAndSlide().
/// Este sistema atua como a ponte "de saída" do ECS para o motor de física.
/// </summary>
// TODO: [UpdateInGroup(typeof(SimulationSystemGroup))]
// GARANTE que rode DEPOIS que a velocidade foi calculada pelo InputApplySystem.
// [UpdateAfter(typeof(InputApplySystem))] 
public partial class InputPhysicsSystem(World world) : BaseSystem<World, float>(world)
{
    [Query]
    [All<SceneBodyRefComponent>]
    private void PrepareAndMove(in SceneBodyRefComponent body, in VelocityComponent vel)
    {
        // Verificação de segurança para evitar crash se o nó foi liberado por algum motivo.
        if (body.Value == null || !Godot.GodotObject.IsInstanceValid(body.Value))
        {
            return;
        }

        // 1) Define a velocidade desejada no corpo físico.
        // O CharacterBody2D usará este valor como sua intenção de movimento.
        body.Value.Velocity = vel.Value;

        // 2) Executa o passo de física.
        // Esta é a chamada mais importante. O Godot irá mover o corpo, detectar
        // colisões, deslizar ao longo de paredes e atualizar seu estado interno.
        body.Value.MoveAndSlide();
    }
}
