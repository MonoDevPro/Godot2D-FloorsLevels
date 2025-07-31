using Arch.Core;
using Game.Shared.Scripts.ECS;
using Game.Shared.Scripts.ECS.Components;
using Game.Shared.Scripts.Entities;
using Game.Shared.Scripts.Entities.Data;
using Game.Shared.Scripts.Network;
using Game.Shared.Scripts.Network.Data.Join;
using Game.Shared.Scripts.Network.Data.Left;
using Game.Shared.Scripts.Network.Transport;
using Godot;
using LiteNetLib;

namespace GodotFloorLevels.Scripts.Root;

public partial class PlayerSpawner : Node2D
{
    [Export] private NodePath _networkPath;
    [Export] private NodePath _ecsPath;

    private NetworkManager _networkManager;
    private EcsRunner _ecsRunner;
    
    private World World => _ecsRunner.World;
    private NetworkReceiver Receiver => _networkManager.Receiver;
    private NetworkSender Sender => _networkManager.Sender;
    
    private readonly Dictionary<int, GodotCharacter2D> _players = new();
    private readonly Dictionary<int, Entity> _entities = new();
    
    public override void _Ready()
    {
        _networkManager = GetNode<NetworkManager>(_networkPath);
        _ecsRunner = GetNode<EcsRunner>(_ecsPath);
        
        Receiver.RegisterMessageHandler<PlayerData>(PlayerDataReceived);
        Receiver.RegisterMessageHandler<LeftResponse>(PlayerLeftReceived);

        _networkManager.PeerRepository.PeerDisconnected += OnPeerDisconnected;
        
        GD.Print("[PlayerSpawner] Ready - Player spawning logic can be initialized here.");
    }
    
    private void OnPeerDisconnected(NetPeer peer, string reason)
    {
        // Change Scenes... Client Disconnected
    }
    
    private void PlayerDataReceived(PlayerData packet, NetPeer peer)
    {
        // Create a new player entity and add it to the scene
        var player = CreatePlayer(ref packet);

        if (packet.NetId == peer.RemoteId)
            World.Add(player.EntityECS, new PlayerControllerTag());
        else
            World.Add(player.EntityECS, new RemoteProxyTag());
    }
    
    private void PlayerLeftReceived(LeftResponse packet, NetPeer peer)
    {
        // Handle player left request
        if (RemovePlayer(packet.NetId))
        {
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
