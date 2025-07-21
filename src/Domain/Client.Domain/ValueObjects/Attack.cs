using GodotFloorLevels.Scripts.Domain.Common;

namespace Client.Domain.ValueObjects;

public readonly record struct Attack(
    int Damage,
    int Range,
    int AttackSpeed,
    int Cooldown) : IValueObject;