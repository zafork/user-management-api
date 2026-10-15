using UserManagement.Application.DTOs;

namespace UserManagement.Application.Ports.In;

public interface IListUsersUseCase
{
    Task<PagedResult<UserDto>> ExecuteAsync(ListUsersQuery query, CancellationToken cancellationToken = default);
}