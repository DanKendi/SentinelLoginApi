namespace SentinelApi.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using SentinelApi.Application.DTOs;
using SentinelApi.Application.UseCases.Auth;
using FluentValidation;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterUserUseCase _registerUserUseCase;
    private readonly LoginUserUseCase _loginUserUseCase;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthController(
        RegisterUserUseCase registerUserUseCase,
        LoginUserUseCase loginUserUseCase,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _registerUserUseCase = registerUserUseCase;
        _loginUserUseCase = loginUserUseCase;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    /// <summary>Cadastra um novo usuário.</summary>
    /// <response code="201">Usuário criado com sucesso. Retorna o idToken.</response>
    /// <response code="400">Dados inválidos ou e-mail já cadastrado.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var validation = await _registerValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(new ProblemDetails
            {
                Title = "Dados inválidos.",
                Detail = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage))
            });

        var response = await _registerUserUseCase.ExecuteAsync(request);
        return CreatedAtAction(nameof(Register), response);
    }

    /// <summary>Autentica um usuario e retorna o idToken Firebase.</summary>
    /// <response code="200">Login realizado com sucesso.</response>
    /// <response code="400">Dados invalidos.</response>
    /// <response code="401">E-mail ou senha incorretos.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var validation = await _loginValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(new ProblemDetails
            {
                Title = "Dados inválidos.",
                Detail = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage))
            });

        var response = await _loginUserUseCase.ExecuteAsync(request);
        return Ok(response);
    }
}