using Game.Shared.Scripts.Core.ValueObjects.Transforms;

namespace Game.Shared.Scripts.Core.ValueObjects.Actions;

public readonly record struct MovementAction(Position TargetPosition, Speed Speed, TimeSpan Duration, Direction Direction)
{
    public MovementAction(Position targetPosition, Speed speed, TimeSpan duration)
        : this(targetPosition, speed, duration, targetPosition.ToDirection())
    {
        if (duration < TimeSpan.Zero) throw new ArgumentException(null, nameof(duration));
    }
}
