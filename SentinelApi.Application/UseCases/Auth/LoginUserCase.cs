namespace SentinelApi.Application.UseCases.Auth;

using SentinelApi.Application.DTOs;
using SentinelApi.Application.Interfaces;
using SentinelApi.Domain.Interfaces;

public class LoginUserUseCase
{
    private readonly IFirebaseAuthService _firebaseAuthService;
    private readonly IUsuarioRepository _usuarioRepository;

    public LoginUserUseCase(
        IFirebaseAuthService firebaseAuthService,
        IUsuarioRepository usuarioRepository)
    {
        _firebaseAuthService = firebaseAuthService;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<AuthResponse> ExecuteAsync(LoginRequest request)
    {
        // Implementação completa na Fase 3
        throw new NotImplementedException();
    }
}