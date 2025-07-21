using Client.Domain.Enums;
using GodotFloorLevels.Scripts.Domain.Common;

namespace Client.Domain.ValueObjects;

public readonly record struct Movement(
    int Speed, 
    Direction Direction) : IValueObject;
