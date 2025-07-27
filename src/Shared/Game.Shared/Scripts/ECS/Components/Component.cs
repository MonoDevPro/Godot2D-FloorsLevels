using Godot;

namespace Game.Shared.Scripts.ECS.Components;

// Core data components used by both client and server
public struct InputCommandComponent { public Vector2 Input;         }
public struct PositionComponent     { public Vector2 Position;      }
public struct VelocityComponent     { public Vector2 Velocity;      }
public struct SpeedComponent        { public float Speed;           }
public struct SceneBodyRefComponent { public CharacterBody2D Node;  }

// Network Components
public struct NetworkedTag             {      public int Id;        }
