using Client.Domain.Enums;

namespace Client.Application.Requests;

public record MovePlayerRequest(Guid CharacterId, DirectionEnum Direction);
