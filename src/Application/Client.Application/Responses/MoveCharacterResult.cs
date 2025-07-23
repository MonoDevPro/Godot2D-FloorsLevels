using Client.Domain.ValueObjects.Transforms;

namespace Client.Application.Responses;

public record MoveCharacterResult(bool Success, Position NewPosition, string? ErrorMessage);
