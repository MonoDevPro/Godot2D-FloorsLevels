using Godot;
using GodotFloorLevels.Scripts.Godot.Config;

namespace GodotFloorLevels.Scripts.Godot.Scenes.Game;

/// <summary>
/// Exemplo de uso do sistema ECS com Godot para movimentação baseada em grid 32x32
/// </summary>
public partial class GameMap : Node2D
{
    [Export] public int GridSize { get; set; } = GameConstants.GridSize; // Tamanho de cada célula do grid em pixels
    [Export] public Vector2I GlobalMapSize { get; set; } = new(25, 25); // Tamanho do mapa em células do grid
    
    public override void _Ready()
    {
        SetupMap();
    }
    
    private void SetupMap()
    {
        GD.Print("Game iniciado com jogador ECS no grid 32x32");
        GD.Print("Use WASD ou setas direcionais para mover o personagem");
    }
}
