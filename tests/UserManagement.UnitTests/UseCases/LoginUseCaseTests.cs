using Moq;
using UserManagement.Application.DTOs;
using UserManagement.Application.Exceptions;
using UserManagement.Application.Ports.Out;
using UserManagement.Application.UseCases;
using UserManagement.Domain.Entities;

namespace UserManagement.UnitTests.UseCases;

public sealed class LoginUseCaseTests
{
    private static readonly DateTime Now = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<ITokenGenerator> _tokenGenerator = new();
    private readonly LoginUseCase _sut;

    public LoginUseCaseTests()
    {
        _sut = new LoginUseCase(_userRepository.Object, _passwordHasher.Object, _tokenGenerator.Object);
    }

    [Fact]
    public async Task Devuelve_el_token_bearer_cuando_las_credenciales_son_correctas()
    {
        var user = User.Create("ana@demo.com", "Ana", "hash-guardado", Now);
        _userRepository.Setup(r => r.GetByEmailAsync("ana@demo.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify("Secreta123!", "hash-guardado")).Returns(true);
        _tokenGenerator.Setup(t => t.Generate(user)).Returns(new AccessToken("token-jwt", 3600));

        var response = await _sut.ExecuteAsync(new LoginRequest { Email = " ANA@demo.com", Password = "Secreta123!" });

        Assert.Equal("token-jwt", response.AccessToken);
        Assert.Equal("Bearer", response.TokenType);
        Assert.Equal(3600, response.ExpiresIn);
    }

    [Fact]
    public async Task Lanza_InvalidCredentials_si_el_usuario_no_existe()
    {
        _userRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            _sut.ExecuteAsync(new LoginRequest { Email = "nadie@demo.com", Password = "x" }));

        _tokenGenerator.Verify(t => t.Generate(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Lanza_InvalidCredentials_si_la_contrasena_es_incorrecta()
    {
        var user = User.Create("ana@demo.com", "Ana", "hash-guardado", Now);
        _userRepository.Setup(r => r.GetByEmailAsync("ana@demo.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            _sut.ExecuteAsync(new LoginRequest { Email = "ana@demo.com", Password = "incorrecta" }));
    }

    [Fact]
    public async Task Lanza_InvalidCredentials_si_el_usuario_esta_inactivo()
    {
        var user = User.Create("ana@demo.com", "Ana", "hash-guardado", Now);
        user.Update(user.FullName, user.Email, isActive: false, Now);
        _userRepository.Setup(r => r.GetByEmailAsync("ana@demo.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            _sut.ExecuteAsync(new LoginRequest { Email = "ana@demo.com", Password = "Secreta123!" }));

        _tokenGenerator.Verify(t => t.Generate(It.IsAny<User>()), Times.Never);
    }
}