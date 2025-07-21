using Client.Domain.Enums;
using Client.Domain.Helpers;
using GodotFloorLevels.Scripts.Domain.Common;

namespace Client.Domain.ValueObjects;

public readonly record struct GridPosition(int X, int Y) : IValueObject
{
    public GridPosition Up() => new(X, Y - 1);
    public GridPosition Down() => new(X, Y + 1);
    public GridPosition Left() => new(X - 1, Y);
    public GridPosition Right() => new(X + 1, Y);
    
    public int DistanceTo(GridPosition other)
        => Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    
    public Direction ToDirection() => DirectionHelper.GridVectorToDirection(this);
    
    public override string ToString() => $"({X}, {Y})";
    
    public static GridPosition Zero => new(0, 0);
    public static GridPosition FromDirection(Direction dir) => DirectionHelper.DirectionToGridVector(dir);
}