using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Game.Shared.Scripts.ECS.Components;

namespace GameServer.Scripts.Root.ECS.Systems.Physics.In;

public partial class ServerInputPhysicsSystem(World world) : BaseSystem<World, float>(world)
{
    [Query]
    [All<SceneBodyRefComponent, VelocityComponent>]
    private void MoveEntities([Data] in float delta, ref SceneBodyRefComponent body, in VelocityComponent vel)
    {
        // 1) Atualiza a Velocity do CharacterBody2D
        body.Node.Velocity = vel.Velocity;
        
        // 2) Executa o passo de física (colisões + deslizamento)
        body.Node.MoveAndSlide();
    }
}