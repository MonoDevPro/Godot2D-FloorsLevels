using Game.Shared.Scripts.Entities;
using Game.Shared.Scripts.Entities.Data;
using Godot;
using GodotFloorLevels.Scripts.Entities;
using GodotFloorLevels.Scripts.Root.ECS;

namespace GodotFloorLevels.Scripts.Root;

public partial class PlayerSpawner : Node2D
{
    [Export] private PackedScene _playerScene;
    
    private readonly Dictionary<int, GodotCharacter2D> _players = new();
    
    public override void _Ready()
    {
        GD.Print("[PlayerSpawner] Ready - Player spawning logic can be initialized here.");
    }
    
    public PlayerBody CreatePlayer(int id)
    {
        // This method would typically retrieve player data from a database or configuration.
        // For simplicity, we return a new instance of PlayerResource here.
        var playerData = new PlayerResource
        {
            Name = $"Player {id}",
            Id = id,
            IsClient = true, // Set to true if this is a client player
            IsLocalPlayer = true,
        };
        
        //return character;
        var playerEntity = PlayerBody.CreatePlayer(playerData, ClientECS.Instance.World);
        
        _players[id] = playerEntity; // Store the player entity by ID
        GD.Print($"[PlayerSpawner] Created player entity for ID: {id} with data: {playerData}");
        
        AddChild(playerEntity);
        
        return playerEntity;
    }
    
    private bool RemovePlayer(int id)
    {
        if (_players.TryGetValue(id, out var playerEntity))
        {
            playerEntity.QueueFree(); // Remove the player entity from the scene
            _players.Remove(id); // Remove from the dictionary
            GD.Print($"[PlayerSpawner] Removed player entity for ID: {id}");
            return true;
        }
        
        GD.PrintErr($"[PlayerSpawner] No player entity found for ID: {id}");
        return false;
    }
}
