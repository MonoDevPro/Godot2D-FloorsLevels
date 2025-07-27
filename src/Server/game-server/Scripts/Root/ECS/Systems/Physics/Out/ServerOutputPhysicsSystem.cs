using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Game.Shared.Scripts.ECS.Components;

namespace GameServer.Scripts.Root.ECS.Systems.Physics.Out;

public partial class ServerOutputPhysicsSystem(World world) : BaseSystem<World, float>(world)
{
    [Query]
    [All<SceneBodyRefComponent, PositionComponent, VelocityComponent>]
    private void SyncPhysicsState(
        [Data] in float delta,
        ref SceneBodyRefComponent body,
        ref PositionComponent pos,
        ref VelocityComponent vel)
    {
        // 1) Atualiza a posição oficial no ECS
        pos.Position   = body.Node.GlobalPosition;
        
        // 2) Atualiza a velocidade real (pode ter sido alterada por colisão)
        vel.Velocity   = body.Node.Velocity;
    }
}