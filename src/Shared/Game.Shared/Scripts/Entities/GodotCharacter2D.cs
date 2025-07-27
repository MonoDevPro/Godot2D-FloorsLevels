using Arch.Core;
using Game.Shared.Config;
using Game.Shared.Scripts.ECS.Components;
using Game.Shared.Scripts.Entities.Data;
using Godot;

namespace Game.Shared.Scripts.Entities;

/// <summary>
/// Classe base para entidades Godot que se integram com o ECS
/// </summary>
public partial class GodotCharacter2D : CharacterBody2D
{
    [Export] protected PlayerResource PlayerResource { get; set; }
    public Entity EntityECS { get; protected set; }
    public World WorldECS { get; protected set; }
    
    private CollisionShape2D _collisionShape;
    
    public static GodotCharacter2D Create(PlayerResource playerResource, World world)
    {
        var character = ResourcePaths.Scenes.Entities.GodotBody.Load().Instantiate<GodotCharacter2D>();
        
        character.PlayerResource = playerResource;
        character.WorldECS = world;
        
        return character;
    }
    
    public override void _Ready()
    {
        base._Ready();
        base.SetProcess(false); // ECS não usa _Process, então desabilitamos para evitar conflitos
        base.SetPhysicsProcess(true); // ECS reaproveita a lógica de física, então habilitamos o processamento físico
        base.SetProcessInput(false); // ECS não usa _Input, então desabilitamos para evitar conflitos
        base.SetProcessUnhandledInput(false); // ECS não usa _UnhandledInput, então desabilitamos para evitar conflitos
        
        // Inicializa o CollisionShape2D
        _collisionShape = GetNode<CollisionShape2D>(nameof(CollisionShape2D));
        
        EntityECS = WorldECS.Create(
            new NetworkedTag { Id = PlayerResource.Id },
            new PositionComponent { Position = PlayerResource.Position },
            new VelocityComponent { Velocity = PlayerResource.Velocity },
            new SceneBodyRefComponent { Node = this },
            new SpeedComponent { Speed = PlayerResource.Speed }
        );
        
        // Adiciona o componente de InputCommandComponent se for o jogador local ou se não for cliente
        // Isso permite que o sistema de entrada do ECS processe os comandos de entrada corretamente
        if (PlayerResource.IsLocalPlayer && PlayerResource.IsClient || !PlayerResource.IsClient)
            WorldECS.Add<InputCommandComponent>(EntityECS, new InputCommandComponent());
    }

    public override void _ExitTree()
    {
        WorldECS.Destroy(EntityECS);
        base._ExitTree();
    }
}
