using Client.Domain.Common;
using Client.Domain.Enums;
using Client.Domain.ValueObjects.Transforms;

namespace Client.Domain.ValueObjects.Actions;

/// <summary>
/// Dados para ação de ficar ocioso.
/// </summary>
public readonly record struct IdleAction : IValueObject
{
    /// <summary>Velocidade de movimento quando o personagem está ocioso.</summary>
    public Speed Speed { get; }
    
    /// <summary>Tempo que o personagem ficará ocioso.</summary>
    public TimeSpan Duration { get; }

    public IdleAction(Speed speed, TimeSpan duration)
    {
        if (duration < TimeSpan.Zero) throw new ArgumentException(null, nameof(duration));
        Speed = speed;
        Duration = duration;
    }
    
    public static IdleAction Default => new(new Speed(), TimeSpan.Zero);
}
