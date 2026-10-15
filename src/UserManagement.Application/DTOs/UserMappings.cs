using UserManagement.Domain.Entities;

namespace UserManagement.Application.DTOs;

public static class UserMappings
{
    public static UserDto ToDto(this User user) => new(
        user.Id,
        user.Email,
        user.FullName,
        user.IsActive,
        user.Roles.Select(r => r.Name).ToList(),
        user.CreatedAt,
        user.UpdatedAt);
}