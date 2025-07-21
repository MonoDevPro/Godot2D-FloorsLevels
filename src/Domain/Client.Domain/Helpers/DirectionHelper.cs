using Client.Domain.Enums;
using Client.Domain.ValueObjects;

namespace Client.Domain.Helpers
{
    /// <summary>
    /// Helper para converter entre GridPosition, ângulos e enumeração Direction no domínio.
    /// Remove dependência da Godot.
    /// </summary>
    public static class DirectionHelper
    {
        /// <summary>
        /// Converte ângulo em graus para direção (8 direções)
        /// </summary>
        public static Direction AngleToDirection(float degrees, bool includeCardinalOnly = false)
        {
            if (includeCardinalOnly)
            {
                return degrees switch
                {
                    >= 315 or < 45 => Direction.East,
                    >= 45 and < 135 => Direction.South,
                    >= 135 and < 225 => Direction.West,
                    >= 225 and < 315 => Direction.North,
                    _ => Direction.None
                };
            }

            return degrees switch
            {
                >= 337.5f or < 22.5f => Direction.East,
                >= 22.5f and < 67.5f => Direction.SouthEast,
                >= 67.5f and < 112.5f => Direction.South,
                >= 112.5f and < 157.5f => Direction.SouthWest,
                >= 157.5f and < 202.5f => Direction.West,
                >= 202.5f and < 247.5f => Direction.NorthWest,
                >= 247.5f and < 292.5f => Direction.North,
                >= 292.5f and < 337.5f => Direction.NorthEast,
                _ => Direction.None
            };
        }

        /// <summary>
        /// Converte um vetor discreto (GridPosition) para direção (8 direções).
        /// Usado para entrada discreta no grid.
        /// </summary>
        public static Direction GridVectorToDirection(GridPosition vector)
        {
            if (vector.X == 0 && vector.Y == 0)
                return Direction.None;

            var clampedX = Math.Clamp(vector.X, -1, 1);
            var clampedY = Math.Clamp(vector.Y, -1, 1);

            return (clampedX, clampedY) switch
            {
                (0, -1) => Direction.North,
                (1, -1) => Direction.NorthEast,
                (1, 0) => Direction.East,
                (1, 1) => Direction.SouthEast,
                (0, 1) => Direction.South,
                (-1, 1) => Direction.SouthWest,
                (-1, 0) => Direction.West,
                (-1, -1) => Direction.NorthWest,
                _ => Direction.None
            };
        }

        /// <summary>
        /// Converte vetor discreto (GridPosition) para direção cardinal apenas (4 direções).
        /// </summary>
        public static Direction GridVectorToCardinalDirection(GridPosition vector)
        {
            if (vector.X == 0 && vector.Y == 0)
                return Direction.None;

            var clampedX = Math.Clamp(vector.X, -1, 1);
            var clampedY = Math.Clamp(vector.Y, -1, 1);

            if (Math.Abs(clampedX) > Math.Abs(clampedY))
                return clampedX > 0 ? Direction.East : Direction.West;
            else if (clampedY != 0)
                return clampedY > 0 ? Direction.South : Direction.North;

            return Direction.None;
        }

        /// <summary>
        /// Converte uma direção para um vetor discreto unitário (GridPosition)
        /// </summary>
        public static GridPosition DirectionToGridVector(Direction direction)
        {
            return direction switch
            {
                Direction.North => new GridPosition(0, -1),
                Direction.NorthEast => new GridPosition(1, -1),
                Direction.East => new GridPosition(1, 0),
                Direction.SouthEast => new GridPosition(1, 1),
                Direction.South => new GridPosition(0, 1),
                Direction.SouthWest => new GridPosition(-1, 1),
                Direction.West => new GridPosition(-1, 0),
                Direction.NorthWest => new GridPosition(-1, -1),
                _ => GridPosition.Zero
            };
        }
    }
}
