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

    /// <summary>
    /// Converte uma posição de célula em coordenada world.
    /// </summary>
    public Vector2 CellToWorld(Vector2I cell)
        => new(cell.X * CellSize.X, cell.Y * CellSize.Y);

    /// <summary>
    /// Posição world atual (baseada em CurrentCell).
    /// </summary>
    public Vector2 WorldPosition => CellToWorld(CurrentCell);

    /// <summary>
    /// Incrementa CurrentCell em direção a TargetCell, respeitando Speed e delta.
    /// Deve ser chamado em _Process(delta) ou no seu System.
    /// </summary>
    public void UpdateMovement(float delta)
    {
        var from = CellToWorld(CurrentCell);
        var to   = CellToWorld(TargetCell);
        var dir  = (to - from);
        var dist = dir.Length();
        if (dist < Mathf.Epsilon) 
            // Já chegou
            return;
        
        var move = SpeedCellsPerSecond * CellSize.Length() * delta;
        var next = (move >= dist)
            ? to
            : from + dir.Normalized() * move;

        // Atualiza a célula atual se passou do centro de outra
        CurrentCell = (Vector2I)next / CellSize;
    }
}