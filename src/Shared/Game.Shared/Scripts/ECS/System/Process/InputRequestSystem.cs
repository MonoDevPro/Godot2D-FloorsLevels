using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Game.Shared.Scripts.ECS.Components;

namespace Game.Shared.Scripts.ECS.System.Process;

/// <summary>
/// Este sistema é o primeiro passo no processamento de input.
/// Ele consome o comando InputRequestCommand e atualiza o estado de InputComponent.
/// Roda tanto no cliente (para predição) quanto no servidor (para autoridade).
/// </summary>
// TODO: [UpdateInGroup(typeof(SimulationSystemGroup))] // Garante que rode no grupo de simulação padrão
public partial class InputRequestSystem(World world) : BaseSystem<World, float>(world)
{
    [Query]
    [All<InputComponent>] // Garante que a entidade possa receber o input
    public void ProcessInputRequest(in Entity entity, ref InputRequestCommand command)
    {
        // Pega o componente de input real
        ref var input = ref World.Get<InputComponent>(entity);
            
        // Atualiza com os dados do comando
        input.Value = command.Value;

        // IMPORTANTE: Remove o comando para que não seja processado novamente no próximo frame
        World.Remove<InputRequestCommand>(entity);
    }
}
