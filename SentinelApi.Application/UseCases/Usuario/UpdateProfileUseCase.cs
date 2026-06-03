namespace SentinelApi.Application.UseCases.Usuario;

using SentinelApi.Application.DTOs;
using SentinelApi.Domain.Exceptions;
using SentinelApi.Domain.Interfaces;

public class UpdateProfileUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UpdateProfileUseCase(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task ExecuteAsync(int idUsuario, UpdateProfileRequest request)
    {
        // Implementação completa na Fase 3
        throw new NotImplementedException();
    }
}