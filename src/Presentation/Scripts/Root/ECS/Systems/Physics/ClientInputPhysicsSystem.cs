using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Game.Shared.Scripts.ECS.Components;

namespace GodotFloorLevels.Scripts.Root.ECS.Systems.Physics;

public partial class ClientInputPhysicsSystem(World world) : BaseSystem<World, float>(world)
{
    [Query] // Client-side movement system: applies InputCommand to velocity
    [All<SceneBodyRefComponent, VelocityComponent, PositionComponent>]
    private void MoveEntities(ref SceneBodyRefComponent scene, ref VelocityComponent vel)
    {
        // 1) Atualiza a Velocity do CharacterBody2D
        scene.Value.Velocity = vel.Value;
        
        // 2) Executa o passo de física (colisões + deslizamento)
        scene.Value.MoveAndSlide();
    }
}
