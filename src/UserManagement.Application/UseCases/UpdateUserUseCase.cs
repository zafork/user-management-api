using UserManagement.Application.DTOs;
using UserManagement.Application.Exceptions;
using UserManagement.Application.Ports.In;
using UserManagement.Application.Ports.Out;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.UseCases;

public sealed class UpdateUserUseCase(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IUpdateUserUseCase
{
    public async Task<UserDto> ExecuteAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"No existe un usuario con id '{userId}'.");

        var email = User.NormalizeEmail(request.Email);
        if (await userRepository.EmailExistsAsync(email, excludeUserId: userId, cancellationToken))
            throw new ConflictException($"El email '{email}' ya esta en uso por otro usuario.");

        user.Update(request.FullName, request.Email, request.IsActive, timeProvider.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToDto();
    }
}