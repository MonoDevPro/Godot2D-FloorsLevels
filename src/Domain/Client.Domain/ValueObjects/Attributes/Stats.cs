using Client.Domain.Common;

namespace Client.Domain.ValueObjects.Attributes;

/// <summary>
/// Conjunto de atributos básicos (força, agilidade, inteligência, endurance).
/// </summary>
public readonly record struct Stats : IValueObject
{
    public int Strength { get; init; }
    public int Agility      { get; init; }
    public int Intelligence { get; init; }
    public int Endurance    { get; init; }

    private const int MinStat = 1;
    private const int MaxStat = 100;

    public Stats(int strength, int agility, int intelligence, int endurance)
    {
        Strength     = Math.Clamp(strength, MinStat, MaxStat);
        Agility      = Math.Clamp(agility, MinStat, MaxStat);
        Intelligence = Math.Clamp(intelligence, MinStat, MaxStat);
        Endurance    = Math.Clamp(endurance, MinStat, MaxStat);
    }

    public Stats IncreaseStrength(int delta)
        => this with { Strength = Math.Clamp(Strength + delta, MinStat, MaxStat) };

    public Stats IncreaseAgility(int delta)
        => this with { Agility = Math.Clamp(Agility + delta, MinStat, MaxStat) };

    public Stats IncreaseIntelligence(int delta)
        => this with { Intelligence = Math.Clamp(Intelligence + delta, MinStat, MaxStat) };

    public Stats IncreaseEndurance(int delta)
        => this with { Endurance = Math.Clamp(Endurance + delta, MinStat, MaxStat) };

    public override string ToString()
        => $"STR:{Strength} AGI:{Agility} INT:{Intelligence} END:{Endurance}";
}
