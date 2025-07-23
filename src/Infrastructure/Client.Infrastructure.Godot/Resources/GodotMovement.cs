using Client.Domain.Constants;
using Client.Infrastructure.Godot.Resources.Constants;
using Godot;

namespace Client.Infrastructure.Godot.Resources;

[GlobalClass]
public partial class GodotMovement : Resource
{
    /// <summary>
    /// Posição atual em células de grade.
    /// </summary>
    [Export] public Vector2I CurrentCell { get; set; } = Vector2I.Zero;
    
    /// <summary>
    /// Célula de destino (onde o personagem quer ir).
    /// </summary>
    [Export] public Vector2I TargetCell { get; set; } = Vector2I.Zero;
    
    /// <summary>
    /// Tamanho de cada célula em pixels (usado para converter grid → mundo).
    /// </summary>
    [Export] public Vector2I CellSize { get; set; } = new(GameConstants.GridSize, GameConstants.GridSize);
    
    /// <summary>
    /// Velocidade em células por segundo.
    /// </summary>
    [Export] public int SpeedCellsPerSecond { get; set; } = GameConstants.DefaultMovementSpeed;
}
