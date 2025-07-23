using Client.Domain.Common;
using Client.Domain.Enums;
using Client.Domain.ValueObjects.Transforms;

namespace Client.Domain.ValueObjects.Actions;

public readonly record struct MovementAction(
    Position TargetPosition,
    Speed Speed,
    TimeSpan Duration,
    DirectionEnum Direction = DirectionEnum.None) : IValueObject
{
    public MovementAction(Position targetPosition, Speed speed, TimeSpan duration)
        : this(targetPosition, speed, duration, targetPosition.ToDirection())
    {
        if (duration < TimeSpan.Zero) throw new ArgumentException(null, nameof(duration));
    }
}
