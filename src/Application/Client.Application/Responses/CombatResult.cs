namespace Client.Application.Responses;

public record CombatResult(Guid AttackerId, Guid DefenderId, int DamageDealt, bool DefenderDied);
