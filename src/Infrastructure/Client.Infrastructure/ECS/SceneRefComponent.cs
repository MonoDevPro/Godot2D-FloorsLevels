using Godot;
using GodotFloorLevels.Scripts.Infrastructure.ArchECS.Components.Common;

namespace Client.Adapters.ECS;

public struct SceneRefComponent       : IComponent { public CharacterBody2D Node; }