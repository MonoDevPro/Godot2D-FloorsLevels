using GodotFloorLevels.Scripts.Infrastructure.ArchECS.Components.Common;

namespace Client.Infrastructure.ECS.Components;

// Tags
public struct PlayerTag               : IComponent { public Guid Id; }
public struct RemotePlayerTag         : IComponent { public Guid Id; }
public struct NpcTag                  : IComponent { public Guid Id; }

// Common Components
public struct PositionComponent       : IComponent { public Vector2I Position; }

public struct VelocityComponent       : IComponent { public Vector2I Velocity; }
public struct SpeedComponent          : IComponent { public float Speed;    }
public struct ChunkRequestComponent   : IComponent { public Vector3I Coord; }
public struct ChunkLoadedTag          : IComponent { }

// Values Objects
public struct Vector3I(int x, int y, int z)
{
    public int X { get; } = x;
    public int Y { get; } = y;
    public int Z { get; } = z;

    public override string ToString() => $"({X}, {Y}, {Z})";
}

public readonly struct Vector2I(int x, int y)
{
    public int X { get; } = x;
    public int Y { get; } = y;

    public override string ToString() => $"({X}, {Y})";
    
    public static Vector2I Zero => new Vector2I(0, 0);
}