namespace Game.Shared.Core.ValueObjects.Actions;

/// <summary>
/// Dados específicos para ação de ataque.
/// </summary>
public readonly record struct AttackAction(int Damage, float Range, TimeSpan Cooldown)
{
    /// <summary>Quantidade de dano causado pelo ataque.</summary>
    public int Damage { get; } = Damage;

    /// <summary>Alcance do ataque em unidades.</summary>
    public float Range { get; } = Range;

    /// <summary>Tempo de recarga do ataque.</summary>
    public TimeSpan Cooldown { get; } = Cooldown;
}
