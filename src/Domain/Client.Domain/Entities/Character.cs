using Client.Domain.Common;
using Client.Domain.Enums;
using Client.Domain.ValueObjects.Actions;
using Client.Domain.ValueObjects.Attributes;
using Client.Domain.ValueObjects.Names;
using Client.Domain.ValueObjects.States;
using Client.Domain.ValueObjects.Transforms;

namespace Client.Domain.Entities;

public class Character(
    Name name,
    VocationEnum vocation,
    GenderEnum genderEnum)
    : DomainEntity
{
    public Name Name { get; private set; } = name;
    public VocationEnum Vocation { get; private set; } = vocation;
    public GenderEnum Gender { get; private set; } = genderEnum;
    public Position Position { get; private set; }
    public Size Size { get; private set; }
    public Stats Stats   { get; private set; }
    public Health Health { get; private set; }
    public CharacterAction CharacterAction { get; private set; }
    public GameState GameState { get; private set; }
}
