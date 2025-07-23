using Client.Domain.Enums;
using Client.Domain.ValueObjects.Actions;
using Client.Domain.ValueObjects.Attributes;
using Client.Domain.ValueObjects.States;
using Client.Domain.ValueObjects.Transforms;
using Client.Infrastructure.ECS.Components.Common;

namespace Client.Infrastructure.ECS.Components;

// Tags
public struct CharacterTag            : IComponent { public Guid Id; }

// Common Components
public struct PositionComponent       : IComponent { public Position Position; }
public struct SizeComponent           : IComponent { public Size Size; }
public struct SpeedComponent          : IComponent { public Speed Speed; }
public struct DirectionComponent      : IComponent { public DirectionEnum Direction; }
public struct HealthComponent         : IComponent { public Health Health; }
public struct StatsComponent          : IComponent { public Stats Stats; }
public struct GameStateComponent      : IComponent { public GameState GameState; }
public struct CharacterActionComponent: IComponent { public CharacterAction CharacterAction; }
