namespace SentinelApi.Application.UseCases.Auth;

using SentinelApi.Application.DTOs;
using SentinelApi.Application.Interfaces;
using SentinelApi.Domain.Exceptions;
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
        // Autentica no Firebase — lança DomainException se credenciais inválidas
        var (idToken, uid) = await _firebaseAuthService.SignInAsync(request.Email, request.Senha);

        // Busca dados complementares no Oracle
        var usuario = await _usuarioRepository.GetByUidFirebaseAsync(uid);
        if (usuario is null)
            throw new DomainException("Usuário não encontrado.");

        // Atualiza o FCM Token se foi enviado
        if (!string.IsNullOrEmpty(request.FcmToken) && usuario.FcmToken != request.FcmToken)
        {
            usuario.FcmToken = request.FcmToken;
            await _usuarioRepository.UpdateAsync(usuario);
        }

        return new AuthResponse(idToken, uid, usuario.Nome, usuario.Email);
    }
}