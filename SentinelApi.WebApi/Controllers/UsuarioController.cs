namespace SentinelApi.WebApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SentinelApi.Application.DTOs;
using SentinelApi.Application.UseCases.Usuario;
using FluentValidation;

[ApiController]
[Route("api/usuario")]
[Authorize]
public class UsuarioController : ControllerBase
{
    private readonly UpdateProfileUseCase _updateProfileUseCase;
    private readonly IValidator<UpdateProfileRequest> _validator;

    public UsuarioController(
        UpdateProfileUseCase updateProfileUseCase,
        IValidator<UpdateProfileRequest> validator)
    {
        _updateProfileUseCase = updateProfileUseCase;
        _validator = validator;
    }

    /// <summary>Atualiza localização e preferências do usuário autenticado.</summary>
    /// <response code="204">Perfil atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    [HttpPut("perfil")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AtualizarPerfil([FromBody] UpdateProfileRequest request)
    {
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(new ProblemDetails
            {
                Title = "Dados inválidos.",
                Detail = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage))
            });

        // Extrai o UID e o ID do usuário a partir do token JWT do Firebase
        var uidFirebase = User.FindFirst("user_id")?.Value
                       ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(uidFirebase))
            return Unauthorized();

        var usuario = await GetUsuarioIdByUid(uidFirebase);
        if (usuario is null)
            return Unauthorized();

        await _updateProfileUseCase.ExecuteAsync(usuario.Value, uidFirebase, request);
        return NoContent();
    }

    // Método auxiliar — busca o ID numérico do Oracle pelo UID do Firebase
    private async Task<int?> GetUsuarioIdByUid(string uid)
    {
        // Resolve via repositório injetado no controller
        var repo = HttpContext.RequestServices
            .GetRequiredService<SentinelApi.Domain.Interfaces.IUsuarioRepository>();

        var usuario = await repo.GetByUidFirebaseAsync(uid);
        return usuario?.IdUsuario;
    }
}