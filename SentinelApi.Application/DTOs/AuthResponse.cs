namespace SentinelApi.Application.DTOs;

public record AuthResponse(
    string IdToken,
    string Uid,
    string Nome,
    string Email
);