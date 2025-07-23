namespace Client.Application.Requests;

public record StartCombatRequest(Guid AttackerId, Guid DefenderId);
