using Client.Domain.Enums;
using GodotFloorLevels.Scripts.Domain.Common;

namespace Client.Domain.ValueObjects;

public readonly record struct Sprite(
    Vocation Vocation, 
    Gender Gender) : IValueObject;