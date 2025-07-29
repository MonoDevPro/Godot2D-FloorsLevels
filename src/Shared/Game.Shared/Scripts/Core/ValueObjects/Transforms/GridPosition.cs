using System.Numerics;
using Game.Shared.Scripts.Core.Enums;

namespace Game.Shared.Scripts.Core.ValueObjects.Transforms;

public readonly record struct GridPosition(Position Position, Grid Grid)
{
    #region Properties
    public int X => Position.X;
    public int Y => Position.Y;
    public bool IsValid => Grid.IsValidPosition(Position);
    #endregion

    #region Grid-Aware Movement
    public GridPosition? North() 
    {
        var newPos = Position.North();
        return Grid.IsValidPosition(newPos) ? new GridPosition(newPos, Grid) : null;
    }

    public GridPosition? South() 
    {
        var newPos = Position.South();
        return Grid.IsValidPosition(newPos) ? new GridPosition(newPos, Grid) : null;
    }

    public GridPosition? East() 
    {
        var newPos = Position.East();
        return Grid.IsValidPosition(newPos) ? new GridPosition(newPos, Grid) : null;
    }

    public GridPosition? West() 
    {
        var newPos = Position.West();
        return Grid.IsValidPosition(newPos) ? new GridPosition(newPos, Grid) : null;
    }

    public GridPosition? Move(DirectionEnum direction)
    {
        var newPos = Position.Move(direction);
        return Grid.IsValidPosition(newPos) ? new GridPosition(newPos, Grid) : null;
    }

    public GridPosition? MoveTo(Position targetPosition)
    {
        return Grid.IsValidPosition(targetPosition) ? new GridPosition(targetPosition, Grid) : null;
    }
    #endregion

    #region Grid Operations
    public IEnumerable<GridPosition> GetValidNeighbors(bool includeDiagonals = false)
    {
        GridPosition position = this;
        return Grid.GetNeighbors(Position, includeDiagonals)
            .Select(pos => new GridPosition(pos, position.Grid));
    }

    public IEnumerable<GridPosition> GetPositionsInRange(int range, bool includeDiagonals = false)
    {
        GridPosition position = this;
        return Grid.GetPositionsInRange(Position, range, includeDiagonals)
            .Select(pos => new GridPosition(pos, position.Grid));
    }

    public int DistanceTo(GridPosition other)
    {
        if (!Grid.Equals(other.Grid))
            throw new ArgumentException("Positions must be from the same grid");
        return Position.DistanceTo(other.Position);
    }

    public double EuclideanDistanceTo(GridPosition other)
    {
        if (!Grid.Equals(other.Grid))
            throw new ArgumentException("Positions must be from the same grid");
        return Position.EuclideanDistanceTo(other.Position);
    }
    #endregion

    #region Conversions
    public int ToIndex() => Grid.PositionToIndex(Position);
    public Vector2 ToWorldPosition(float cellSize) => Grid.GridToWorld(Position, cellSize);
        
    public DirectionEnum ToDirection() => Position.ToDirection();
    public DirectionEnum ToCardinalDirection() => Position.ToCardinalDirection();
    #endregion

    #region Clamping and Wrapping
    public GridPosition Clamp() => new(Grid.ClampPosition(Position), Grid);
    public GridPosition Wrap() => new(Grid.WrapPosition(Position), Grid);
    #endregion

    #region Operators and Conversions
    public static implicit operator Position(GridPosition gridPos) => gridPos.Position;
    public static explicit operator GridPosition((Position pos, Grid grid) tuple) 
        => new(tuple.pos, tuple.grid);

    public static GridPosition operator +(GridPosition gridPos, Position offset)
    {
        var newPos = gridPos.Position + offset;
        return new GridPosition(newPos, gridPos.Grid);
    }

    public static GridPosition operator -(GridPosition gridPos, Position offset)
    {
        var newPos = gridPos.Position - offset;
        return new GridPosition(newPos, gridPos.Grid);
    }
    #endregion

    #region Factory Methods
    public static GridPosition? Create(int x, int y, Grid grid)
    {
        var pos = new Position(x, y);
        return grid.IsValidPosition(pos) ? new GridPosition(pos, grid) : null;
    }

    public static GridPosition CreateUnsafe(int x, int y, Grid grid)
        => new(new Position(x, y), grid);

    public static GridPosition? FromIndex(int index, Grid grid)
    {
        var pos = grid.IndexToPosition(index);
        return grid.IsValidPosition(pos) ? new GridPosition(pos, grid) : null;
    }

    public static GridPosition? FromWorldPosition(Vector2 worldPos, float cellSize, Grid grid)
    {
        var pos = grid.WorldToGrid(worldPos, cellSize);
        return grid.IsValidPosition(pos) ? new GridPosition(pos, grid) : null;
    }
    #endregion

    public override string ToString() => $"GridPos({X}, {Y}) in {Grid}";
}
