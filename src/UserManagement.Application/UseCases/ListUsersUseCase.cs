using UserManagement.Application.DTOs;
using UserManagement.Application.Ports.In;
using UserManagement.Application.Ports.Out;

namespace UserManagement.Application.UseCases;

public sealed class ListUsersUseCase(IUserRepository userRepository) : IListUsersUseCase
{
    public async Task<PagedResult<UserDto>> ExecuteAsync(ListUsersQuery query, CancellationToken cancellationToken = default)
    {
        var result = await userRepository.ListAsync(query.Page, query.PageSize, cancellationToken);

        return new PagedResult<UserDto>(
            result.Items.Select(user => user.ToDto()).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount);
    }
}