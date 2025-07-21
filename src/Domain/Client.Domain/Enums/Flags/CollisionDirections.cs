namespace Client.Domain.Enums.Flags;

/// <summary>
/// Flags para indicar quais direções estão bloqueadas
/// </summary>
[Flags]
public enum CollisionDirections : byte
{
    None = 0,
    North = 1 << 0,
    South = 1 << 1,
    East = 1 << 2,
    West = 1 << 3,
    NorthEast = 1 << 4,
    NorthWest = 1 << 5,
    SouthEast = 1 << 6,
    SouthWest = 1 << 7
}