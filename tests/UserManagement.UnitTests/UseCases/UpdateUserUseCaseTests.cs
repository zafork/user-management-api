using Moq;
using UserManagement.Application.DTOs;
using UserManagement.Application.Exceptions;
using UserManagement.Application.Ports.Out;
using UserManagement.Application.UseCases;
using UserManagement.Domain.Entities;
using UserManagement.UnitTests.Fakes;

namespace UserManagement.UnitTests.UseCases;

public sealed class UpdateUserUseCaseTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdateUserUseCase _sut;

    public UpdateUserUseCaseTests()
    {
        _sut = new UpdateUserUseCase(_userRepository.Object, _unitOfWork.Object, new FixedTimeProvider(Now));
    }

    private static UpdateUserRequest Request(string email = "ana@demo.com") =>
        new() { FullName = "Ana María", Email = email, IsActive = true };

    [Fact]
    public async Task Lanza_NotFound_si_el_usuario_no_existe()
    {
        _userRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.ExecuteAsync(Guid.NewGuid(), Request()));
    }

    [Fact]
    public async Task Lanza_Conflict_si_el_email_pertenece_a_otro_usuario()
    {
        var user = User.Create("ana@demo.com", "Ana", "hash", Now.UtcDateTime);
        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _userRepository.Setup(r => r.EmailExistsAsync("luis@demo.com", user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() => _sut.ExecuteAsync(user.Id, Request("luis@demo.com")));

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Actualiza_el_usuario_y_guarda_los_cambios()
    {
        var user = User.Create("ana@demo.com", "Ana", "hash", Now.UtcDateTime.AddDays(-10));
        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _sut.ExecuteAsync(user.Id, Request());

        Assert.Equal("Ana María", result.FullName);
        Assert.Equal(Now.UtcDateTime, result.UpdatedAt);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}