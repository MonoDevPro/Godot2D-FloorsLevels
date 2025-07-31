using Arch.Core;
using Arch.System;
using Game.Shared.Scripts.Network;
using Game.Shared.Scripts.Network.Transport;

namespace Game.Shared.Scripts.ECS.NetworkSync;

/// <summary>
/// Sistema que processa mensagens recebidas da rede e aplica ao ECS
/// Executa PRIMEIRO na pipeline para garantir que dados de rede sejam processados antes da lógica
/// </summary>
public abstract class LambdaNetReceiveSystem(World world, NetworkManager networkManager) : BaseSystem<World, float>(world)
{
    private readonly List<IDisposable> _subscriptions = [];
    
    public override void Initialize()
    {
        _subscriptions.AddRange(RegisterNetworkHandlers(networkManager.Receiver));
    }

    public override void Update(in float t)
    {
        // PollEvents() é o método principal que dispara os callbacks (como o OnPlayerInputReceived)
        // O primeiro parâmetro (listener) é nulo porque estamos usando o NetPacketProcessor
        // que atua como nosso listener para pacotes de dados.
        networkManager.PollEvents();
    }
    
    protected abstract List<IDisposable> RegisterNetworkHandlers(NetworkReceiver receiver);
    
    public override void Dispose()
    {
        // Desregistra todos os handlers
        foreach (var subscription in _subscriptions)
        {
            subscription.Dispose();
        }
        _subscriptions.Clear();
        
        base.Dispose();
    }
}
