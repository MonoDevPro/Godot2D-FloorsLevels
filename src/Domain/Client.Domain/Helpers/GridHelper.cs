using Client.Domain.ValueObjects;

namespace Client.Domain.Helpers;

public static class GridHelper
{
    public static bool IsInsideGrid(GridPosition pos, int width, int height) =>
        pos is { X: >= 0, Y: >= 0 } && pos.X < width && pos.Y < height;

    public static IEnumerable<GridPosition> GetNeighbors(GridPosition pos)
    {
        yield return pos.Up();
        yield return pos.Down();
        yield return pos.Left();
        yield return pos.Right();
    }
}
