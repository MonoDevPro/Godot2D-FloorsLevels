using Client.Domain.Entities;

namespace Client.Domain.Interfaces;

public interface ICharacterStore
{
    /// <summary>
    /// Resolves the player entity by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the player entity.</param>
    /// <returns>The character entity associated with the given ID.</returns>
    Character GetCharacter(Guid id);
}