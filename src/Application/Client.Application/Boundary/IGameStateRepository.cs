using Client.Domain.ValueObjects.States;

namespace Client.Application.Boundary;

public interface IGameStateRepository
{
    Task<GameState> LoadAsync(string slot);
    Task SaveAsync(GameState state, string slot);
}
