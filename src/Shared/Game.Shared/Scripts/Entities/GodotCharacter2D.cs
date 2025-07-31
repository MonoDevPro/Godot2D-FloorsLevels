using Godot;
using Arch.Core;
using Game.Shared.Resources;
using Game.Shared.Scripts.ECS.Components;
using Game.Shared.Scripts.Entities.Data;

namespace Game.Shared.Scripts.Entities;

/// <summary>
/// Classe base para entidades Godot que se integram com o ECS
/// </summary>
public partial class GodotCharacter2D : CharacterBody2D
{
    public PlayerData PlayerData { get; private set; }
    public Entity EntityECS { get; private set; }
    public World WorldECS { get; private set; }
    
    private CollisionShape2D _collisionShape;
    private AnimatedSprite2D _animatedSprite;
    
    public static GodotCharacter2D Create(ref PlayerData playerData)
    {
        var character = ResourcePaths.Scenes.Entities.GodotBody.Load().Instantiate<GodotCharacter2D>();
        character.PlayerData = playerData;
        return character;
    }

    public Entity CreateEntityECS(World world)
    {
        WorldECS = world;
        
        EntityECS = WorldECS.Create(
            new NetworkedTag { Id = PlayerData.NetId },
            new PositionComponent { Value = PlayerData.Position },
            new VelocityComponent { Value = PlayerData.Velocity },
            new SpeedComponent { Value = PlayerData.Speed },
            new InputComponent { Value = Vector2.Zero },
            new SceneBodyRefComponent { Value = this }
        );
            

        return EntityECS;
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
        
        // Client Only: Inicializa o AnimatedSprite2D
        var spriteNode = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        if (spriteNode is not null)
        {
            _animatedSprite = spriteNode;
            _animatedSprite.SpriteFrames = ResourcePaths.Resources.GetSpriteFrames(
                PlayerData.Vocation, PlayerData.Gender).Load();
            
            _animatedSprite.Play();
        }
    }

    public override void _ExitTree()
    {
        WorldECS.Destroy(EntityECS);
        base._ExitTree();
    }
}
