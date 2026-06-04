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

    public async Task ExecuteAsync(int idUsuario, string uidFirebase, UpdateProfileRequest request)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
        if (usuario is null)
            throw new DomainException("Usuário não encontrado.");

        // Garante que apenas o próprio usuário edita o próprio perfil
        if (usuario.UidFirebase != uidFirebase)
            throw new DomainException("Acesso negado.");

        usuario.Latitude = request.Latitude;
        usuario.Longitude = request.Longitude;
        usuario.RaioKm = request.RaioKm;

        await _usuarioRepository.UpdateAsync(usuario);
    }
}