using Arch.Core;
using Arch.System;
using Game.Shared.ECS;
using Game.Shared.ECS.Components;
using Game.Shared.ECS.Systems;
using Godot;

namespace GameServer;

public partial class ServerECS : EcsRunner
{
    private readonly QueryDescription SyncQuery = new QueryDescription()
        .WithAll<PositionComponent>();
    
    protected override void OnCreateSystems(List<ISystem<float>> systems)
    {
        systems.AddRange(
            [
                new NpcAISystem(World)
            ]
        );
        
        base.OnCreateSystems(systems);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        // Broadcast positions of all entities to clients
        World.Query<PositionComponent>(SyncQuery))
       
        
        {
            Rpc("RpcUpdateState", entity.Id.ToString(), pos.Position);
        }
    }
    
    [Rpc(MultiplayerApi.RpcMode.Authority)]
    public void RpcSendInput(string peerId, Vector2 input)
    {
        // Find entity by peerId and apply InputCommandComponent
        var entity = /* lookup by RemoteTag(PeerId) */;
        World.Set(entity, new InputCommandComponent { Input = input });
    }
}