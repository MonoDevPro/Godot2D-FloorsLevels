using Arch.Core;
using Client.Domain.ValueObjects;
using Client.Domain.ValueObjects.Attributes;
using Client.Domain.ValueObjects.Transforms;
using Client.Infrastructure.ECS.Components;
using Client.Infrastructure.ECS.Entities.Common;
using Microsoft.Extensions.Logging;

namespace Client.Infrastructure.ECS.Entities;

public sealed class NpcECS : EntityECS
{
    // Factory
    public static NpcECS Create(WorldECS worldECS, ILogger? log = null)
    {
        var world = worldECS.World;
        var e = world.Create();
        var inst = new NpcECS(world, e, log);
        inst.Logger?.LogInformation("Criada entidade {EntityId} do tipo {EntityType}", e.Id, nameof(NpcECS));
        inst.RegisterComponents();
        return inst;
    }
    
    private NpcECS(
        World world, 
        Entity entity, 
        ILogger? log = null) 
        : base(world, entity, log) { }
    
    protected override void RegisterComponents()
    {
        Logger?.LogTrace("Registrando componentes do Npc ECS.");
        
        base.RegisterComponents();
        
        // Registra os componentes do jogador local
        base.AddComponent(new PositionComponent { Position = new Position() });
        base.AddComponent(new SpeedComponent { Speed = new Speed() });
        base.AddComponent(new NpcTag());
        
        QueryDescription
            .WithExclusive<NpcTag>()
            .WithAll<PositionComponent, SpeedComponent>()
            .WithNone<RemotePlayerTag, CharacterTag >();
    }
}
