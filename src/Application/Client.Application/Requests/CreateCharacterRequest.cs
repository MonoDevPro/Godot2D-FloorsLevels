using Client.Domain.Enums;
using Client.Domain.ValueObjects.Names;

namespace Client.Application.Requests;

public record CreatePlayerRequest(Name Name, VocationEnum Vocation, GenderEnum Gender);
