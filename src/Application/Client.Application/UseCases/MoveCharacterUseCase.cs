namespace Client.Application.UseCases;

public class MoveCharacterUseCase
{
    /*
     Execute(MovePlayerRequest request):
    • Obtém a entidade via `IEcsContext.Query(...)`
    • Calcula nova posição: `Position.Normalized() + Velocity`
    • Atualiza componente `PositionComponent`
    • Presenta `MovePlayerResult`
    */
}
