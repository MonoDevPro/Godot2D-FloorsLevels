using Client.Application.Requests;

namespace Client.Application.UseCases;

public class CreateCharacterUseCase
{
    /*
     Execute(CreatePlayerRequest request):
    • Valida o `request.Name`
    • Chama `IPlayerRepository.SaveAsync()`
    • Cria entidade ECS: `EcsContext.CreateEntity() + AddComponent<PlayerComponent>`  
    • `OutputPort.Present(CreatePlayerResponse)`
    */
}

