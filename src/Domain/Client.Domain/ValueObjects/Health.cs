using GodotFloorLevels.Scripts.Domain.Common;

namespace Client.Domain.ValueObjects;

public readonly record struct Health(
    int MaxHealth, 
    int CurrentHealth) : IValueObject;