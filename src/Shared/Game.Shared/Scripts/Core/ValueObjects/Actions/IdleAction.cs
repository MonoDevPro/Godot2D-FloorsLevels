using Game.Shared.Scripts.Core.ValueObjects.Transforms;

namespace Game.Shared.Scripts.Core.ValueObjects.Actions;

/// <summary>
/// Dados para ação de ficar ocioso.
/// </summary>
public readonly record struct IdleAction(Speed Speed, TimeSpan Duration)
{
    /// <summary>Velocidade de movimento quando o personagem está ocioso.</summary>
    public Speed Speed { get; } = Speed;

    /// <summary>Tempo que o personagem ficará ocioso.</summary>
    public TimeSpan Duration { get; } = Duration;

    public static IdleAction Default => new(new Speed(), TimeSpan.Zero);
}
