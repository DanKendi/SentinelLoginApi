namespace SentinelApi.Application.DTOs;

public record LoginRequest(
    string Email,
    string Senha,
    string? FcmToken = null
);