namespace SentinelApi.Application.DTOs;

public record RegisterRequest(
    string Nome,
    string Email,
    string Senha,
    string FcmToken,
    int RaioKm
);