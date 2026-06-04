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
        // Verifica se e-mail já existe no Oracle
        var existente = await _usuarioRepository.GetByEmailAsync(request.Email);
        if (existente is not null)
            throw new DomainException("E-mail já cadastrado.");

        // Cria o usuário no Firebase e obtém o UID
        var uid = await _firebaseAuthService.CreateUserAsync(
            request.Email,
            request.Senha,
            request.Nome
        );

        // Persiste no Oracle com o UID do Firebase
        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email.ToLower().Trim(),
            SenhaHash = request.Senha, // A senha já vem do request — Firebase gerencia o hash
            FcmToken = request.FcmToken,
            UidFirebase = uid,
            RaioKm = request.RaioKm,
            DataCadastro = DateTime.UtcNow,
            Ativo = 'S'
        };

        await _usuarioRepository.AddAsync(usuario);

        // Faz login imediatamente após o cadastro para retornar o idToken
        var (idToken, _) = await _firebaseAuthService.SignInAsync(request.Email, request.Senha);

        return new AuthResponse(idToken, uid, usuario.Nome, usuario.Email);
    }
}