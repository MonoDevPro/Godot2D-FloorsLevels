using Godot;

namespace Game.Shared.ECS.Components;

// Core data components used by both client and server
public struct PositionComponent { public Vector2 Position; }
public struct VelocityComponent { public Vector2 Velocity; }
public struct SpeedComponent    { public float Speed;    }
public struct SceneRefComponent { public CharacterBody2D Node; }
public struct LocalTag          { }
public struct RemoteTag         { public string PeerId; }
public struct NPCPatrolTag      { public Vector2[] PatrolPoints; public int CurrentIndex; }
public struct FloorComponent    { public int Floor; }
public struct InputCommandComponent { public Vector2 Input; }
