using Arch.Core;
using Game.Shared.Scripts.ECS;
using Game.Shared.Scripts.Entities;
using Game.Shared.Scripts.Entities.Data;
using Game.Shared.Scripts.Network;
using Game.Shared.Scripts.Network.Data.Join;
using Game.Shared.Scripts.Network.Data.Left;
using Game.Shared.Scripts.Network.Transport;
using Godot;
using LiteNetLib;

namespace GameServer.Scripts.Root;

public partial class PlayerSpawner : Node2D
{
    [Export] private NodePath _networkPath;
    [Export] private NodePath _ecsPath;

    private NetworkManager _networkManager;
    private EcsRunner _ecsRunner;
    
    private World World => _ecsRunner.World;
    private NetworkReceiver Receiver => _networkManager.Receiver;
    public NetworkSender Sender => _networkManager.Sender;
    public NetworkManager NetworkManager => _networkManager;
    
    private readonly Dictionary<int, GodotCharacter2D> _players = new();
    private readonly Dictionary<int, Entity> _entities = new();
    
    public override void _Ready()
    {
        _networkManager = GetNode<NetworkManager>(_networkPath);
        _ecsRunner = GetNode<EcsRunner>(_ecsPath);
        
        Receiver.RegisterMessageHandler<JoinRequest>(RequestPlayerJoin);
        Receiver.RegisterMessageHandler<LeftRequest>(RequestPlayerLeft);

        _networkManager.PeerRepository.PeerDisconnected += OnPeerDisconnected;
        
        GD.Print("[PlayerSpawner] Ready - Player spawning logic can be initialized here.");
    }
    
    private void OnPeerDisconnected(NetPeer peer, string reason)
    {
        // Handle player disconnection
        GD.Print($"[PlayerSpawner] Player Disconnected with ID: {peer.Id} reason: {reason}");
        
        // Optionally, you can send a left request to the server
        var leftRequest = new LeftRequest();
        // Remove the player from the scene
        RequestPlayerLeft(leftRequest, peer);
    }
    
    private void RequestPlayerJoin(JoinRequest packet, NetPeer peer)
    {
        // Load Player Data
        var playerData = new PlayerData
        {
            Name = packet.Name,
            NetId = peer.Id,
        };
        // Create a new player entity and add it to the scene
        var player = CreatePlayer(ref playerData);
        
        // Optionally, you can send a confirmation back to the client
        Sender.Broadcast(ref playerData);
    }
    
    private void RequestPlayerLeft(LeftRequest packet, NetPeer peer)
    {
        // Handle player left request
        if (RemovePlayer(peer.Id))
        {
            // Optionally, you can send a confirmation back to the client
            var leftResponse = new LeftResponse() { NetId = peer.Id};
            Sender.Broadcast(ref leftResponse);

            GD.Print($"[PlayerSpawner] Player Left with ID: {peer.Id}");
        }
    }
    
    public bool TryGetPlayerById(int netId, out GodotCharacter2D playerEntity)
    {
        if (_players.TryGetValue(netId, out playerEntity) 
            && World.IsAlive(playerEntity.EntityECS))
            return true; // Player entity found
        
        GD.PrintErr($"[PlayerSpawner] No player entity found for ID: {netId}");
        return false; // Player entity not found
    }
    public bool TryGetEntityById(int entityId, out Entity entity)
    {
        if (_entities.TryGetValue(entityId, out entity) 
            && World.IsAlive(entity))
            return true; // entity found
        
        GD.PrintErr($"[PlayerSpawner] No entity found for ID: {entityId}");
        return false; // entity not found
    }
    
    private GodotCharacter2D CreatePlayer(ref PlayerData data)
    {
        var player = GodotCharacter2D.Create(ref data);
        var entity = player.CreateEntityECS(World);
        
        _players[data.NetId] = player; // Store the player entity by ID
        _entities[data.NetId] = entity; // Store the ECS entity by ID
        
        AddChild(player);
        GD.Print($"[PlayerSpawner] Created player with data: {data}");
        
        return player;
    }
    
    private bool RemovePlayer(int id)
    {
        if (!_players.Remove(id, out var player))
        {
            GD.PrintErr($"[PlayerSpawner] No player entity found for ID: {id}");
            return false; // Player entity not found
        }
        _entities.Remove(id); // Remove from the entities dictionary
        player.QueueFree(); // Remove the player entity from the scene
        
        GD.Print($"[PlayerSpawner] Removed player entity for ID: {id}");
        return true;
    }
}
