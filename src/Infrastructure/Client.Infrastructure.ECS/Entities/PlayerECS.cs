using Arch.Core;
using Client.Domain.ValueObjects;
using Client.Domain.ValueObjects.Attributes;
using Client.Domain.ValueObjects.Transforms;
using Client.Infrastructure.ECS.Components;
using Client.Infrastructure.ECS.Entities.Common;
using Microsoft.Extensions.Logging;

namespace Client.Infrastructure.ECS.Entities;

public sealed class PlayerECS : EntityECS
{
    // Factory
    public static PlayerECS Create(WorldECS worldECS, ILogger? log = null)
    {
        var world = worldECS.World;
        var e = world.Create();
        var inst = new PlayerECS(world, e, log);
        inst.Logger?.LogInformation("Criada entidade {EntityId} do tipo {EntityType}", e.Id, nameof(PlayerECS));
        inst.RegisterComponents();
        return inst;
    }
    
    private PlayerECS(
        World world, 
        Entity entity, 
        ILogger? log = null) 
        : base(world, entity, log) { }
    
    protected override void RegisterComponents()
    {
        Logger?.LogTrace("Registrando componentes do jogador ECS.");
        
        base.RegisterComponents();
        
        // Registra os componentes do jogador local
        base.AddComponent(new PositionComponent { Position = new Position() });
        base.AddComponent(new SpeedComponent { Speed = new Speed() });
        base.AddComponent(new PlayerInputComponent { Input = new PlayerInput() });
        base.AddComponent(new CharacterTag());
        
        QueryDescription
            .WithExclusive<CharacterTag, PlayerInputComponent>()
            .WithAll<PositionComponent, SpeedComponent>()
            .WithNone<RemotePlayerTag, NpcTag>();
    }
}
