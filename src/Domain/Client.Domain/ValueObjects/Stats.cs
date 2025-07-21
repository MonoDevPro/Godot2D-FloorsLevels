using GodotFloorLevels.Scripts.Domain.Common;

namespace Client.Domain.ValueObjects;

/// <summary>
/// Conjunto de atributos básicos (força, agilidade, inteligência etc.).
/// </summary>
public readonly record struct Stats(
    int Strength, 
    int Agility, 
    int Intelligence, 
    int Endurance) : IValueObject;
