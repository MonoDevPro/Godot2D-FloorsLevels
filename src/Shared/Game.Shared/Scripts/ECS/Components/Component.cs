using System.Runtime.CompilerServices;
using Arch.LowLevel;
using Godot;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.ECS.Components;

// Core data components used by both client and server
public struct PositionSyncCommand { public Vector2 Value;     }
public struct VelocitySyncCommand { public Vector2 Value;     }
