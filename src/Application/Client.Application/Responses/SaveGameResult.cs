namespace Client.Application.Responses;

public record SaveGameResult(bool Success, DateTimeOffset Timestamp, string? ErrorMessage);
