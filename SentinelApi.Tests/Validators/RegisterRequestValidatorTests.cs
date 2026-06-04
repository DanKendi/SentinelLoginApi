namespace SentinelApi.Tests.Validators;

using FluentAssertions;
using SentinelApi.Application.DTOs;
using SentinelApi.Application.Validators;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public async Task Validate_DevePassar_QuandoDadosValidos()
    {
        // Arrange
        var request = new RegisterRequest(
            Nome: "João Silva",
            Email: "joao@email.com",
            Senha: "senha123",
            FcmToken: "fcm_token_joao",
            RaioKm: 100
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_DeveFalhar_QuandoEmailInvalido()
    {
        // Arrange
        var request = new RegisterRequest(
            Nome: "João Silva",
            Email: "email-invalido",
            Senha: "senha123",
            FcmToken: "fcm_token_joao",
            RaioKm: 100
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Validate_DeveFalhar_QuandoSenhaMenorQue6Caracteres()
    {
        // Arrange
        var request = new RegisterRequest(
            Nome: "João Silva",
            Email: "joao@email.com",
            Senha: "123",
            FcmToken: "fcm_token_joao",
            RaioKm: 100
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Senha");
    }

    [Fact]
    public async Task Validate_DeveFalhar_QuandoRaioNegativo()
    {
        // Arrange
        var request = new RegisterRequest(
            Nome: "João Silva",
            Email: "joao@email.com",
            Senha: "senha123",
            FcmToken: "fcm_token_joao",
            RaioKm: -10
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RaioKm");
    }

    [Fact]
    public async Task Validate_DeveFalhar_QuandoRaioAcimaDoLimite()
    {
        // Arrange
        var request = new RegisterRequest(
            Nome: "João Silva",
            Email: "joao@email.com",
            Senha: "senha123",
            FcmToken: "fcm_token_joao",
            RaioKm: 999
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RaioKm");
    }
}