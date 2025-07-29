using Game.Shared.Scripts.Core.Enums;

namespace Game.Shared.Scripts.Core.ValueObjects.Transforms
{
    public readonly record struct Position(int X, int Y)
    {
        #region Cardinal Directions
        public Position North() => new(X, Y - 1);
        public Position South() => new(X, Y + 1);
        public Position West()  => new(X - 1, Y);
        public Position East()  => new(X + 1, Y);
        #endregion

        #region Intercardinal Directions
        public Position NorthEast() => North().East();
        public Position NorthWest() => North().West();
        public Position SouthEast() => South().East();
        public Position SouthWest() => South().West();
        #endregion

        #region Transformations
        public static Position Zero() => new(0, 0);
        public Position Opposite()   => new(-X, -Y);
        public Position Inverted()   => new(Y, X);
        public Position Absolute()   => new(Math.Abs(X), Math.Abs(Y));
        public Position Normalized() => new(Math.Clamp(X, -1, 1), Math.Clamp(Y, -1, 1));
        #endregion

        #region Distance Calculations
        public int DistanceTo(Position other) 
            => Math.Abs(X - other.X) + Math.Abs(Y - other.Y);

        public double EuclideanDistanceTo(Position other) 
            => Math.Sqrt((X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y));
        #endregion

        #region Conversions
        public DirectionEnum ToCardinalDirection()
        {
            if (X == 0 && Y == 0) return DirectionEnum.None;
            var cx = Math.Clamp(X, -1, 1);
            var cy = Math.Clamp(Y, -1, 1);
            return Math.Abs(cx) > Math.Abs(cy)
                ? (cx > 0 ? DirectionEnum.East : DirectionEnum.West)
                : (cy > 0 ? DirectionEnum.South : DirectionEnum.North);
        }

        public DirectionEnum ToDirection() => (Math.Clamp(X,-1,1),Math.Clamp(Y,-1,1)) switch
        {
            ( 0,-1) => DirectionEnum.North,
            ( 1,-1) => DirectionEnum.NorthEast,
            ( 1, 0) => DirectionEnum.East,
            ( 1, 1) => DirectionEnum.SouthEast,
            ( 0, 1) => DirectionEnum.South,
            (-1, 1) => DirectionEnum.SouthWest,
            (-1, 0) => DirectionEnum.West,
            (-1,-1) => DirectionEnum.NorthWest,
            _       => DirectionEnum.None
        };

        public static Position ToPositionNormalized(float x, float y)
        {
            var normX = Math.Clamp((int)Math.Round(x), -1, 1);
            var normY = Math.Clamp((int)Math.Round(y), -1, 1);
            return new Position(normX, normY);
        }

        public override string ToString() => $"({X}, {Y})";
        #endregion

        #region Operators
        public static Position operator +(Position a, Position b) => new(a.X + b.X, a.Y + b.Y);
        public static Position operator -(Position a, Position b) => new(a.X - b.X, a.Y - b.Y);
        public static Position operator *(Position p, int s)      => new(p.X * s, p.Y * s);
        public static Position operator /(Position p, int s)      => new(p.X / s, p.Y / s);
        #endregion
        
        #region Grid Extensions
        /// <summary>
        /// Verifica se a posição está dentro dos limites do grid
        /// </summary>
        public bool IsWithinBounds(int width, int height) 
            => X >= 0 && X < width && Y >= 0 && Y < height;

        /// <summary>
        /// Obtém todas as posições adjacentes (4 direções cardinais)
        /// </summary>
        public IEnumerable<Position> GetAdjacentPositions()
        {
            yield return North();
            yield return South();
            yield return East();
            yield return West();
        }

        /// <summary>
        /// Obtém todas as posições adjacentes incluindo diagonais (8 direções)
        /// </summary>
        public IEnumerable<Position> GetAllAdjacentPositions()
        {
            yield return North();
            yield return NorthEast();
            yield return East();
            yield return SouthEast();
            yield return South();
            yield return SouthWest();
            yield return West();
            yield return NorthWest();
        }

        /// <summary>
        /// Move na direção especificada
        /// </summary>
        public Position Move(DirectionEnum direction) => direction switch
        {
            DirectionEnum.North => North(),
            DirectionEnum.NorthEast => NorthEast(),
            DirectionEnum.East => East(),
            DirectionEnum.SouthEast => SouthEast(),
            DirectionEnum.South => South(),
            DirectionEnum.SouthWest => SouthWest(),
            DirectionEnum.West => West(),
            DirectionEnum.NorthWest => NorthWest(),
            _ => this
        };

        /// <summary>
        /// Clamp a posição dentro dos limites do grid
        /// </summary>
        public Position ClampToGrid(int width, int height)
            => new(Math.Clamp(X, 0, width - 1), Math.Clamp(Y, 0, height - 1));

        #endregion
    }
}
