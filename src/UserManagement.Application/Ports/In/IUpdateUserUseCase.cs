using UserManagement.Application.DTOs;

namespace UserManagement.Application.Ports.In;

public interface IUpdateUserUseCase
{
    Task<UserDto> ExecuteAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default);
}