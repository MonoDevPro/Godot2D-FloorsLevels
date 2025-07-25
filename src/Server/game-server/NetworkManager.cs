// NetworkManager.cs - Servidor

using Game.Shared.Network.Config;
using Godot;

public partial class NetworkManager : Node
{
    private MultiplayerApi _multiplayer;
    private readonly Dictionary<int, PlayerData> _players = new();
    
    public override void _Ready()
    {
        // Configura o servidor
        var peer = new ENetMultiplayerPeer();
        var error = peer.CreateServer(NetworkConfigurations.Port, NetworkConfigurations.MaxClients);
        
        if (error != Error.Ok)
        {
            GD.PrintErr($"Erro ao criar servidor: {error}");
            return;
        }
        
        _multiplayer = GetTree().GetMultiplayer();
        _multiplayer.MultiplayerPeer = peer;
        
        // Conecta sinais do multiplayer
        _multiplayer.PeerConnected += OnPeerConnected;
        _multiplayer.PeerDisconnected += OnPeerDisconnected;
        
        GD.Print($"Servidor iniciado na porta {NetworkConfigurations.Port}");
    }
    
    private void OnPeerConnected(long id)
    {
        GD.Print($"Cliente conectado: {id}");
        
        // Adiciona novo jogador
        var playerData = new PlayerData
        {
            Id = (int)id,
            Name = $"Player_{id}",
            IsOnline = true
        };
        
        _players[(int)id] = playerData;
        
        // Notifica todos os clientes sobre o novo jogador
        RpcBroadcastPlayerJoined((int)id, playerData.Name);
        
        // Envia lista de jogadores para o novo cliente
        RpcId((int)id, MethodName.ReceivePlayerList, GetPlayerList());
    }
    
    private void OnPeerDisconnected(long id)
    {
        GD.Print($"Cliente desconectado: {id}");
        
        if (_players.ContainsKey((int)id))
        {
            _players.Remove((int)id);
            // Notifica todos sobre a desconexão
            RpcBroadcastPlayerLeft((int)id);
        }
    }
    
    // RPCs que o servidor pode chamar
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void RpcBroadcastPlayerJoined(int playerId, string playerName)
    {
        // Este método será chamado nos clientes
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void RpcBroadcastPlayerLeft(int playerId)
    {
        // Este método será chamado nos clientes
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void RpcBroadcastMessage(int senderId, string senderName, string message)
    {
        // Este método será chamado nos clientes
    }
    
    // RPCs que o servidor recebe dos clientes
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void ReceiveMessage(string message)
    {
        int senderId = _multiplayer.GetRemoteSenderId();
        
        if (_players.ContainsKey(senderId))
        {
            var playerData = _players[senderId];
            GD.Print($"Mensagem de {playerData.Name}: {message}");
            
            // Retransmite para todos os clientes
            RpcBroadcastMessage(senderId, playerData.Name, message);
        }
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void UpdatePlayerName(string newName)
    {
        int playerId = _multiplayer.GetRemoteSenderId();
        
        if (_players.ContainsKey(playerId))
        {
            _players[playerId].Name = newName;
            GD.Print($"Jogador {playerId} mudou nome para: {newName}");
            
            // Notifica todos sobre a mudança
            RpcUpdatePlayerInfo(playerId, newName);
        }
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void RpcUpdatePlayerInfo(int playerId, string newName)
    {
        // Este método será chamado nos clientes
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void ReceivePlayerList(Godot.Collections.Array<Godot.Collections.Dictionary> playerList)
    {
        // Este método é chamado apenas nos clientes
    }
    
    private Godot.Collections.Array<Godot.Collections.Dictionary> GetPlayerList()
    {
        var playerList = new Godot.Collections.Array<Godot.Collections.Dictionary>();
        
        foreach (var player in _players.Values)
        {
            var playerDict = new Godot.Collections.Dictionary
            {
                ["id"] = player.Id,
                ["name"] = player.Name,
                ["isOnline"] = player.IsOnline
            };
            playerList.Add(playerDict);
        }
        
        return playerList;
    }
    
    public override void _ExitTree()
    {
        if (_multiplayer?.MultiplayerPeer != null)
        {
            _multiplayer.MultiplayerPeer.Close();
        }
    }
}

// PlayerData.cs
public class PlayerData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsOnline { get; set; }
}