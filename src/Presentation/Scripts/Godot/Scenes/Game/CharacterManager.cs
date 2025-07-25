using Godot;
using GodotFloorLevels.Scripts.Godot.Entities.Common;
using GodotCharacter2D = GodotFloorLevels.Scripts.Godot.Entities.GodotCharacter2D;

namespace GodotFloorLevels.Scripts.Godot.Scenes.Game;

// CharacterManager.cs

[Tool]
public partial class CharacterManager : Node
{
    // Signals to notify the rest of the game when a display is added or removed
    [Signal] public delegate void CharacterAddedEventHandler(int characterId, CharacterBody2D characterNode);
    [Signal] public delegate void CharacterRemovedEventHandler(int characterId);

    // Stores logical data (e.g., state/AI) and visual nodes
    private Dictionary<int, object> _characterData = new();
    private Dictionary<int, CharacterBody2D> _characterDisplays = new();

    /// <summary>
    /// Registers or updates a character (local player, remote player, NPC).
    /// If it doesn't exist yet, instantiates the display and emits CharacterAdded.
    /// </summary>
    public void RegisterCharacter(int characterId, GodotCharacter2D godotCharacter, Vector2 position, int floor)
    {
        // Logical data placeholder (replace 'object' with your PlayerData/NPCData classes)
        if (!_characterData.ContainsKey(characterId))
            _characterData[characterId] = new object();

        // Visual display
        if (!_characterDisplays.TryGetValue(characterId, out CharacterBody2D display1))
        {
            var display = (CharacterBody2D)characterScene.Instantiate();
            display.Name = $"Character_{characterId}";
            _characterDisplays[characterId] = display;

            // Parent and position on the correct floor
            var floorNode = GetNode<Node2D>("../MapLayer/MapRoot/ChunkManager/Floor_" + floor);
            floorNode.AddChild(display);
            display.GlobalPosition = position;

            EmitSignal(SignalName.CharacterAdded, characterId, display);
        }
        else
        {
            // Already exists: reposition or change floor
            MoveDisplayTo(display1, position, floor);
        }
    }

    /// <summary>
    /// Removes the character display and data.
    /// </summary>
    public void RemoveCharacter(string characterId)
    {
        if (_characterDisplays.TryGetValue(characterId, out var display))
        {
            display.QueueFree();
            _characterDisplays.Remove(characterId);
            EmitSignal(Scenes.CharacterManager.SignalName.CharacterRemoved, characterId);
        }
        _characterData.Remove(characterId);
    }

    private void MoveDisplayTo(CharacterBody2D display, Vector2 newPos, int newFloor)
    {
        var oldParent = display.GetParent();
        var newFloorNode = GetNode<Node2D>("../MapLayer/MapRoot/ChunkManager/Floor_" + newFloor);
        if (oldParent != newFloorNode)
        {
            oldParent.RemoveChild(display);
            newFloorNode.AddChild(display);
        }
        display.GlobalPosition = newPos;
    }

    /// <summary>
    /// Gets the display node by ID, or null if not found.
    /// </summary>
    public CharacterBody2D GetDisplay(string characterId)
        => _characterDisplays.TryGetValue(characterId, out var d) ? d : null;

    /// <summary>
    /// Enumerates all active display nodes.
    /// </summary>
    public IEnumerable<CharacterBody2D> GetAllDisplays() => _characterDisplays.Values;
}
