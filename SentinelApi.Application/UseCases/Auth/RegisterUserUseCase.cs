namespace SentinelApi.Application.UseCases.Auth;

using SentinelApi.Application.DTOs;
using SentinelApi.Application.Interfaces;
using SentinelApi.Domain.Entities;
using SentinelApi.Domain.Exceptions;
using SentinelApi.Domain.Interfaces;

public class RegisterUserUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IFirebaseAuthService _firebaseAuthService;

    public RegisterUserUseCase(
        IUsuarioRepository usuarioRepository,
        IFirebaseAuthService firebaseAuthService)
    {
        _usuarioRepository = usuarioRepository;
        _firebaseAuthService = firebaseAuthService;
    }

    public async Task<AuthResponse> ExecuteAsync(RegisterRequest request)
    {
        // Implementação completa na Fase 3
        throw new NotImplementedException();
    }
}