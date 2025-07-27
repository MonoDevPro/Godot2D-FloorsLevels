using Game.Shared.Core.Enums;

namespace Game.Shared.Core.ValueObjects.Transforms;

/// <summary>
/// Value Object que encapsula uma direção, com validações e conversões.
/// </summary>
public readonly record struct Direction(DirectionEnum Value)
{
    /// <summary>
    /// Verifica se a direção é cartesiana (N, S, E, W) sem diagonais.
    /// </summary>
    public bool IsCardinal => Value switch
    {
        DirectionEnum.North   or
            DirectionEnum.South   or
            DirectionEnum.East    or
            DirectionEnum.West    => true,
        _ => false
    };

    /// <summary>
    /// Converte para um vetor de deslocamento unitário (X,Y).
    /// </summary>
    public Position ToOffset() => Value switch
    {
        DirectionEnum.North     => new Position(0, -1),
        DirectionEnum.NorthEast => new Position(1, -1),
        DirectionEnum.East      => new Position(1, 0),
        DirectionEnum.SouthEast => new Position(1, 1),
        DirectionEnum.South     => new Position(0, 1),
        DirectionEnum.SouthWest => new Position(-1, 1),
        DirectionEnum.West      => new Position(-1, 0),
        DirectionEnum.NorthWest => new Position(-1, -1),
        _ => new Position(0, 0)
    };
    
    public static implicit operator DirectionEnum(Direction direction) 
        => direction.Value;
    
    public static implicit operator Direction(DirectionEnum directionEnum) 
        => new(directionEnum);

    /// <summary>
    /// Tenta criar um VO a partir de um vetor de offset (X, Y) normalizado.
    /// </summary>
    public static Direction FromOffset(Position offset)
        => new Direction(offset.ToDirection());

    public override string ToString() => Value.ToString();
}
