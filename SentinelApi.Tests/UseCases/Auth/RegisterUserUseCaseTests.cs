namespace SentinelApi.Tests.UseCases.Auth;

using Moq;
using FluentAssertions;
using SentinelApi.Application.DTOs;
using SentinelApi.Application.Interfaces;
using SentinelApi.Application.UseCases.Auth;
using SentinelApi.Domain.Entities;
using SentinelApi.Domain.Exceptions;
using SentinelApi.Domain.Interfaces;

public class RegisterUserUseCaseTests
{
    // Mocks reutilizados em todos os testes
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IFirebaseAuthService> _firebaseAuthServiceMock;
    private readonly RegisterUserUseCase _sut; // sut = System Under Test

    public RegisterUserUseCaseTests()
    {
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _firebaseAuthServiceMock = new Mock<IFirebaseAuthService>();

        _sut = new RegisterUserUseCase(
            _usuarioRepositoryMock.Object,
            _firebaseAuthServiceMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_DeveRetornarAuthResponse_QuandoDadosValidos()
    {
        // Arrange
        var request = new RegisterRequest(
            Nome: "Maria Teste",
            Email: "maria@teste.com",
            Senha: "senha123",
            FcmToken: "fcm_token_maria",
            RaioKm: 50
        );

        _usuarioRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((Usuario?)null); // e-mail não existe

        _firebaseAuthServiceMock
            .Setup(f => f.CreateUserAsync(request.Email, request.Senha, request.Nome))
            .ReturnsAsync("uid_firebase_123");

        _firebaseAuthServiceMock
            .Setup(f => f.SignInAsync(request.Email, request.Senha))
            .ReturnsAsync(("id_token_fake", "uid_firebase_123"));

        // Act
        var response = await _sut.ExecuteAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Uid.Should().Be("uid_firebase_123");
        response.Email.Should().Be("maria@teste.com");
        response.IdToken.Should().Be("id_token_fake");

        // Verifica que o usuário foi salvo no Oracle exatamente uma vez
        _usuarioRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarDomainException_QuandoEmailJaCadastrado()
    {
        // Arrange
        var request = new RegisterRequest(
            Nome: "Ana Duplicada",
            Email: "ana@email.com",
            Senha: "senha123",
            FcmToken: "fcm_token_ana",
            RaioKm: 50
        );

        _usuarioRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(new Usuario { Email = request.Email }); // e-mail já existe

        // Act
        var act = async () => await _sut.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("E-mail já cadastrado.");

        // Garante que o Firebase nunca foi chamado
        _firebaseAuthServiceMock.Verify(
            f => f.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_NaoDeveSalvarNoOracle_QuandoFirebaseFalha()
    {
        // Arrange
        var request = new RegisterRequest(
            Nome: "Carlos Erro",
            Email: "carlos@teste.com",
            Senha: "senha123",
            FcmToken: "fcm_token_carlos",
            RaioKm: 80
        );

        _usuarioRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((Usuario?)null);

        _firebaseAuthServiceMock
            .Setup(f => f.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new DomainException("Erro ao criar usuário no Firebase."));

        // Act
        var act = async () => await _sut.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<DomainException>();

        // Garante que o Oracle não foi tocado se o Firebase falhou
        _usuarioRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Usuario>()),
            Times.Never);
    }
}