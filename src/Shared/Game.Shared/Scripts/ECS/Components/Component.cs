using Godot;

namespace Game.Shared.Scripts.ECS.Components;

// Core data components used by both client and server
public struct InputCommandComponent { public Vector2 Value;         }
public struct PositionComponent     { public Vector2 Value;      }
public struct VelocityComponent     { public Vector2 Value;      }
public struct SpeedComponent        { public float Value;           }
public struct SceneBodyRefComponent { public CharacterBody2D Value;  }

// Network Components
public struct NetworkedTag             {      public int Id;        }
