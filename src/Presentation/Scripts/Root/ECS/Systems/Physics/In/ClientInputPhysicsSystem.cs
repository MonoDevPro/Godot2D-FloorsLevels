using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Game.Shared.Scripts.ECS.Components;

namespace GodotFloorLevels.Scripts.Root.ECS.Systems.Physics.In;

public partial class ClientInputPhysicsSystem(World world) : BaseSystem<World, float>(world)
{
    [Query] // Client-side movement system: applies InputCommand to velocity
    [All<SceneBodyRefComponent, VelocityComponent, PositionComponent>]
    private void MoveEntities(ref SceneBodyRefComponent scene, ref VelocityComponent vel, ref PositionComponent pos)
    {
        // 1) Atualiza a Velocity do CharacterBody2D
        scene.Node.Velocity = vel.Velocity;
        
        // 2) Executa o passo de física (colisões + deslizamento)
        scene.Node.MoveAndSlide();
    }
}
