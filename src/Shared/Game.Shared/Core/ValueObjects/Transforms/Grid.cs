using System.Numerics;

namespace Game.Shared.Core.ValueObjects.Transforms;

public readonly record struct Grid(Size Size, Grid.GridTypeEnum Type = Grid.GridTypeEnum.Rectangular)
{
    private int Width => Size.Width;
    private int Height => Size.Height;
    
    // 3. Enum para tipos de grid
    public enum GridTypeEnum
    {
        Rectangular,
        Circular,
        Diamond,
        Hexagonal  // Para futuras implementações
    }
    
    #region Properties
    public int TotalCells => Width * Height;
    public Position Center => new(Width / 2, Height / 2);
    public Position TopLeft => Position.Zero();
    public Position TopRight => new(Width - 1, 0);
    public Position BottomLeft => new(0, Height - 1);
    public Position BottomRight => new(Width - 1, Height - 1);
    #endregion

    #region Validation
    public bool IsValidPosition(Position position) => Type switch
    {
        GridTypeEnum.Rectangular => position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height,
        GridTypeEnum.Circular => IsWithinCircle(position),
        GridTypeEnum.Diamond => IsWithinDiamond(position),
        _ => false
    };

    private bool IsWithinCircle(Position position)
    {
        var center = Center;
        var radius = Math.Min(Width, Height) / 2;
        return position.EuclideanDistanceTo(center) <= radius;
    }

    private bool IsWithinDiamond(Position position)
    {
        var center = Center;
        var distance = position.DistanceTo(center);
        var maxDistance = Math.Min(Width, Height) / 2;
        return distance <= maxDistance;
    }
    #endregion

    #region Position Operations
    public Position ClampPosition(Position position) => new(
        Math.Clamp(position.X, 0, Width - 1),
        Math.Clamp(position.Y, 0, Height - 1)
    );

    public Position WrapPosition(Position position) => new(
        ((position.X % Width) + Width) % Width,
        ((position.Y % Height) + Height) % Height
    );

    public IEnumerable<Position> GetAllValidPositions()
    {
        for (int y = 0; y < Height; y++)
        for (int x = 0; x < Width; x++)
        {
            var pos = new Position(x, y);
            if (IsValidPosition(pos))
                yield return pos;
        }
    }

    public IEnumerable<Position> GetBorderPositions()
    {
        for (int x = 0; x < Width; x++)
        {
            yield return new Position(x, 0);           // Top
            yield return new Position(x, Height - 1);  // Bottom
        }
        for (int y = 1; y < Height - 1; y++)
        {
            yield return new Position(0, y);           // Left
            yield return new Position(Width - 1, y);   // Right
        }
    }

    public Position GetRandomPosition(Random random) => Type switch
    {
        GridTypeEnum.Rectangular => new(random.Next(Width), random.Next(Height)),
        _ => GetRandomValidPosition(random)
    };

    private Position GetRandomValidPosition(Random random)
    {
        var validPositions = GetAllValidPositions().ToList();
        return validPositions[random.Next(validPositions.Count)];
    }
    #endregion

    #region Coordinate Conversions
    public int PositionToIndex(Position position)
        => IsValidPosition(position) ? position.Y * Width + position.X : -1;

    public Position IndexToPosition(int index)
        => index >= 0 && index < TotalCells ? new(index % Width, index / Width) : Position.Zero();

    public Position WorldToGrid(Vector2 worldPosition, float cellSize)
        => new((int)(worldPosition.X / cellSize), (int)(worldPosition.Y / cellSize));

    public Vector2 GridToWorld(Position gridPosition, float cellSize)
        => new(gridPosition.X * cellSize, gridPosition.Y * cellSize);
    #endregion

    #region Neighbors and Pathfinding
    public IEnumerable<Position> GetNeighbors(Position position, bool includeDiagonals = false)
    {
        var neighbors = includeDiagonals 
            ? position.GetAllAdjacentPositions() 
            : position.GetAdjacentPositions();
            
        return neighbors.Where(IsValidPosition);
    }

    public IEnumerable<Position> GetPositionsInRange(Position center, int range, bool includeDiagonals = false)
    {
        for (int x = center.X - range; x <= center.X + range; x++)
        {
            for (int y = center.Y - range; y <= center.Y + range; y++)
            {
                var pos = new Position(x, y);
                if (!IsValidPosition(pos) || pos == center) continue;

                var distance = includeDiagonals 
                    ? pos.EuclideanDistanceTo(center) 
                    : pos.DistanceTo(center);

                if (distance <= range)
                    yield return pos;
            }
        }
    }
    #endregion

    #region Grid Operations
    public Grid Resize(Size size)
        => new(size, Type);

    public Grid ChangeType(GridTypeEnum newType)
        => new(Size, newType);

    public override string ToString()
        => $"Grid({Width}x{Height}, {Type})";
    #endregion

    #region Static Factory Methods
    public static Grid Square(int size) => new(new Size(size, size));
    public static Grid Rectangle(int width, int height) => new(new Size(width, height));
    public static Grid Circle(int diameter) => new(new Size(diameter, diameter), GridTypeEnum.Circular);
    public static Grid Diamond(int size) => new(new Size(size, size), GridTypeEnum.Diamond);
    #endregion
}
