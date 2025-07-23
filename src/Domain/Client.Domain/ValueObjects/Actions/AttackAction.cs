using Client.Domain.Common;

namespace Client.Domain.ValueObjects.Actions;

/// <summary>
/// Dados específicos para ação de ataque.
/// </summary>
public readonly record struct AttackAction : IValueObject
{
    /// <summary>Quantidade de dano causado pelo ataque.</summary>
    public int Damage { get; }
    /// <summary>Alcance do ataque em unidades.</summary>
    public float Range { get; }
    /// <summary>Tempo de recarga do ataque.</summary>
    public TimeSpan Cooldown { get; }
    
    public AttackAction(int damage, float range, TimeSpan cooldown)
    {
        if (damage < 0) throw new ArgumentException(null, nameof(damage));
        if (range < 0)  throw new ArgumentException(null, nameof(range));
        if (cooldown < TimeSpan.Zero) throw new ArgumentException(null, nameof(cooldown));
        Damage = damage;
        Range = range;
        Cooldown = cooldown;
    }
}
