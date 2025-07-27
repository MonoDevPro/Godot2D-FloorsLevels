using Game.Shared.Scripts.Entities;
using Game.Shared.Scripts.Entities.Data;
using GameServer.Scripts.Root.ECS;
using GameServer.Scripts.Root.Network;
using Godot;

namespace GameServer.Scripts.Root;

public partial class PlayerSpawner : Node2D
{
    [Export] private PackedScene _playerScene;
    
    private readonly Dictionary<int, GodotCharacter2D> _players = new();
    
    public override void _Ready()
    {
        // This is where you would typically initialize player spawning logic.
        // For example, you might connect to a network event or set up a timer
        // to spawn players at regular intervals.
        
        GD.Print("[PlayerSpawner] Ready - Player spawning logic can be initialized here.");
        ServerNetwork.Instance.PeerRepository.PeerConnected += OnPeerConnected;
        ServerNetwork.Instance.PeerRepository.PeerDisconnected += OnPeerDisconnected;
        
        //CreatePlayer(1);
    }
    
    private GodotCharacter2D CreatePlayer(int id)
    {
        // This method would typically retrieve player data from a database or configuration.
        // For simplicity, we return a new instance of PlayerResource here.
        var playerData = new PlayerResource
        {
            Name = $"Player {id}",
            Id = id,
            IsClient = false, // Set to true if this is a client player
            IsLocalPlayer = false,
        };
        
        // Create a new player entity using the player data
        /*var playerEntity = _playerScene.Instantiate<GodotCharacter2D>();
        
        playerEntity.PlayerResource = playerData; // Assign the player data to the entity
        playerEntity.WorldECS = ServerECS.Instance.World;*/
        
        //return character;
        var playerEntity = GodotCharacter2D.Create(playerData, ServerECS.Instance.World);
        
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
    
    private void OnPeerConnected(int peerId)
    {
        GD.Print($"[PlayerSpawner] Peer connected: {peerId}");
        
        CreatePlayer(peerId);
    }
    
    private void OnPeerDisconnected(int peerId, string reason)
    {
        GD.Print($"[PlayerSpawner] Peer disconnected: {peerId}, Reason: {reason}");
        
        if (!RemovePlayer(peerId))
        {
            GD.PrintErr($"[PlayerSpawner] Failed to remove player entity for peer ID: {peerId}");
        }
    }
}