using Arch.Core;
using Client.Domain.Enums;
using Godot;
using GodotFloorLevels.Scripts.Core.ValueObjects.Transforms;
using GodotFloorLevels.Scripts.Godot.Resources.Utils;

namespace GodotFloorLevels.Scripts.Godot.Entities;

/// <summary>
/// Classe base para entidades Godot que se integram com o ECS
/// </summary>
public partial class GodotCharacter2D : CharacterBody2D
{
    [Export] public CharacterResource CharacterResource { get; set; } = new();
    
    public Entity EcsEntity { get; set; }
    public World EcsWorld { get; set; }
    
    public override void _Ready()
    {
        base._Ready();
        
        AddChild(SpriteResource.ToAnimatedSprite2D());
        
        // Inicializa o AnimatedSprite2D
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _ExitTree()
    {
        if (EcsWorld.IsAlive(EcsEntity))
            EcsWorld.Destroy(EcsEntity);
        
        base._ExitTree();
    }

    protected GridPosition CreateGridPosition(Vector2 worldPosition, int gridWidth = 100, int gridHeight = 100)
    {
        const float TILE_SIZE = 32f;
        var gridPos = new Position(
            Mathf.RoundToInt(worldPosition.X / TILE_SIZE),
            Mathf.RoundToInt(worldPosition.Y / TILE_SIZE)
        );
        var grid = new Grid(new Size(gridWidth, gridHeight));
        return new GridPosition(gridPos, grid);
    }
}
