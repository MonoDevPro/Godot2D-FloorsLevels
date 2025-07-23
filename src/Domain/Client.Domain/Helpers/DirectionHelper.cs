using Client.Domain.Enums;

namespace Client.Domain.Helpers;

/// <summary>
/// Helper para converter entre GridPosition, ângulos e enumeração Direction no domínio.
/// Remove dependência da Godot.
/// </summary>
public static class DirectionHelper
{
    /// <summary>
    /// Converte ângulo em graus para direção (8 direções)
    /// </summary>
    public static DirectionEnum AngleToDirection(float degrees, bool includeCardinalOnly = false)
    {
        if (includeCardinalOnly)
        {
            return degrees switch
            {
                >= 315 or < 45 => DirectionEnum.East,
                >= 45 and < 135 => DirectionEnum.South,
                >= 135 and < 225 => DirectionEnum.West,
                >= 225 and < 315 => DirectionEnum.North,
                _ => DirectionEnum.None
            };
        }

        return degrees switch
        {
            >= 337.5f or < 22.5f => DirectionEnum.East,
            >= 22.5f and < 67.5f => DirectionEnum.SouthEast,
            >= 67.5f and < 112.5f => DirectionEnum.South,
            >= 112.5f and < 157.5f => DirectionEnum.SouthWest,
            >= 157.5f and < 202.5f => DirectionEnum.West,
            >= 202.5f and < 247.5f => DirectionEnum.NorthWest,
            >= 247.5f and < 292.5f => DirectionEnum.North,
            >= 292.5f and < 337.5f => DirectionEnum.NorthEast,
            _ => DirectionEnum.None
        };
    }
}
