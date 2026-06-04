namespace SentinelApi.Tests.UseCases.Auth;

using Moq;
using FluentAssertions;
using SentinelApi.Application.DTOs;
using SentinelApi.Application.Interfaces;
using SentinelApi.Application.UseCases.Auth;
using SentinelApi.Domain.Entities;
using SentinelApi.Domain.Exceptions;
using SentinelApi.Domain.Interfaces;

public class LoginUserUseCaseTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IFirebaseAuthService> _firebaseAuthServiceMock;
    private readonly LoginUserUseCase _sut;

    public LoginUserUseCaseTests()
    {
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _firebaseAuthServiceMock = new Mock<IFirebaseAuthService>();

        _sut = new LoginUserUseCase(
            _firebaseAuthServiceMock.Object,
            _usuarioRepositoryMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_DeveRetornarAuthResponse_QuandoCredenciaisValidas()
    {
        // Arrange
        var request = new LoginRequest("lucas@email.com", "senha123");

        _firebaseAuthServiceMock
            .Setup(f => f.SignInAsync(request.Email, request.Senha))
            .ReturnsAsync(("id_token_valido", "uid_lucas_001"));

        _usuarioRepositoryMock
            .Setup(r => r.GetByUidFirebaseAsync("uid_lucas_001"))
            .ReturnsAsync(new Usuario
            {
                IdUsuario = 1,
                Nome = "Lucas Mendes",
                Email = "lucas@email.com",
                UidFirebase = "uid_lucas_001",
                FcmToken = "fcm_token_lucas"
            });

        // Act
        var response = await _sut.ExecuteAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.IdToken.Should().Be("id_token_valido");
        response.Uid.Should().Be("uid_lucas_001");
        response.Nome.Should().Be("Lucas Mendes");
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarDomainException_QuandoCredenciaisInvalidas()
    {
        // Arrange
        var request = new LoginRequest("errado@email.com", "senhaerrada");

        _firebaseAuthServiceMock
            .Setup(f => f.SignInAsync(request.Email, request.Senha))
            .ThrowsAsync(new DomainException("E-mail ou senha inválidos."));

        // Act
        var act = async () => await _sut.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("E-mail ou senha inválidos.");

        // Oracle não deve ser consultado se o Firebase rejeitou
        _usuarioRepositoryMock.Verify(
            r => r.GetByUidFirebaseAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarDomainException_QuandoUsuarioNaoEncontradoNoOracle()
    {
        // Arrange
        var request = new LoginRequest("fantasma@email.com", "senha123");

        _firebaseAuthServiceMock
            .Setup(f => f.SignInAsync(request.Email, request.Senha))
            .ReturnsAsync(("id_token_ok", "uid_fantasma"));

        _usuarioRepositoryMock
            .Setup(r => r.GetByUidFirebaseAsync("uid_fantasma"))
            .ReturnsAsync((Usuario?)null); // existe no Firebase mas não no Oracle

        // Act
        var act = async () => await _sut.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Usuário não encontrado.");
    }
}