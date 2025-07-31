using Arch.System;
using Game.Shared.Scripts.ECS;
using Game.Shared.Scripts.ECS.NetworkSync;
using Game.Shared.Scripts.Network;
using Godot;

namespace GameServer.Scripts.Root.ECS;

/// <summary>
/// Implementação específica do ECS para o servidor
/// Gerencia a integração entre ArchECS e LiteNetLib no lado servidor
/// </summary>
public partial class ServerECS : EcsRunner
{
    [Export] public NodePath NetworkManagerPath { get; set; }
    private NetworkManager _networkManager;

    public override void _Ready()
    {
        // Primeiro obtém referência do NetworkManager
        _networkManager = GetNode<NetworkManager>(NetworkManagerPath);
        if (_networkManager == null)
        {
            GD.PrintErr("[ServerECS] NetworkManager não encontrado no path especificado!");
            return;
        }

        // Chama o _Ready base que inicializa todos os sistemas
        base._Ready();
        
        GD.Print("[ServerECS] Servidor ECS inicializado com sucesso");
    }

    protected override void OnCreateProcessSystems(List<ISystem<float>> systems)
    {
        // Adicione seus sistemas de processo do servidor aqui
        // Ex: systems.Add(new ServerInputProcessSystem(World, _networkManager));
        // Ex: systems.Add(new GameLogicSystem(World));
        GD.Print("[ServerECS] Sistemas de processo do servidor registrados");
    }

    protected override void OnCreatePhysicsSystems(List<ISystem<float>> systems)
    {
        // Adicione seus sistemas de física do servidor aqui
        // Ex: systems.Add(new ServerPhysicsSystem(World));
        // Ex: systems.Add(new CollisionSystem(World));
        GD.Print("[ServerECS] Sistemas de física do servidor registrados");
    }

    public override void _Process(double delta)
    {
        if (_networkManager?.NetManager.IsRunning == true)
            UpdateProcessSystems((float)delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_networkManager?.NetManager.IsRunning == true)
            UpdatePhysicsSystems((float)delta);
    }
}
