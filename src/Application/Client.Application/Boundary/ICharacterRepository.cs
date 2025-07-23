using Client.Domain.Entities;

namespace Client.Application.Boundary;

public interface ICharacterRepository
{
    Task<Character?> GetByIdAsync(Guid id);
    Task SaveAsync(Character character);
}
