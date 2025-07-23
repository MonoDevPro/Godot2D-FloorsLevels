using Client.Domain.Common;

namespace Client.Domain.ValueObjects.Attributes;

/// <summary>
/// Pontos de vida, com valor entre 0 e Max.
/// </summary>
public readonly record struct Health : IValueObject
{
    public int Current { get; }
    public int Max     { get; }

    public Health(int current, int max)
    {
        if (max <= 0)
            throw new ArgumentException("Max deve ser maior que zero.", nameof(max));
        Max = max;
        Current = Math.Clamp(current, 0, Max);
    }

    /// <summary>
    /// Aplica dano e retorna novo Health.
    /// </summary>
    public Health TakeDamage(int damage)
        => new(Current - Math.Max(damage, 0), Max);

    /// <summary>
    /// Cura e retorna novo Health.
    /// </summary>
    public Health Heal(int amount)
        => new(Current + Math.Max(amount, 0), Max);

    /// <summary>
    /// Indica se está sem vida.
    /// </summary>
    public bool IsDead => Current == 0;
}
