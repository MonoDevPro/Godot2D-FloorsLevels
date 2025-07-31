using Arch.System;
using Game.Shared.Scripts.ECS;
using Game.Shared.Scripts.Network;
using Godot;

namespace GodotFloorLevels.Scripts.Root.ECS;

/// <summary>
/// Implementação específica do ECS para o cliente
/// Gerencia a integração entre ArchECS e LiteNetLib no lado cliente
/// </summary>
public partial class ClientECS : EcsRunner
{
    [Export] public NodePath NetworkManagerPath { get; set; }
    private NetworkManager _networkManager;

    public override void _Ready()
    {
        // Primeiro obtém referência do NetworkManager
        _networkManager = GetNode<NetworkManager>(NetworkManagerPath);
        
        if (_networkManager == null)
        {
            GD.PrintErr("[ClientECS] NetworkManager não encontrado no path especificado!");
            return;
        }

        // Chama o _Ready base que inicializa todos os sistemas
        base._Ready();
        
        GD.Print("[ClientECS] Cliente ECS inicializado com sucesso");
    }

    protected override void OnCreateProcessSystems(List<ISystem<float>> systems)
    {
        // Adicione seus sistemas de processo do cliente aqui
        // Ex: systems.Add(new InputProcessingSystem(World));
        // Ex: systems.Add(new UIUpdateSystem(World));
        GD.Print("[ClientECS] Sistemas de processo do cliente registrados");
    }

    protected override void OnCreatePhysicsSystems(List<ISystem<float>> systems)
    {
        // Adicione seus sistemas de física do cliente aqui
        // Ex: systems.Add(new ClientPredictionSystem(World));
        // Ex: systems.Add(new InterpolationSystem(World));
        GD.Print("[ClientECS] Sistemas de física do cliente registrados");
    }
    
    public override void _Process(double delta)
    {
        if (_networkManager?.NetManager.IsRunning == true)
        {
            UpdateProcessSystems((float)delta);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_networkManager?.NetManager.IsRunning == true)
        {
            UpdatePhysicsSystems((float)delta);
        }
    }
}
