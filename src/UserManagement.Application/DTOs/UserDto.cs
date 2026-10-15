namespace UserManagement.Application.DTOs;

public sealed record UserDto(
    Guid Id,
    string Email,
    string FullName,
    bool IsActive,
    IReadOnlyList<string> Roles,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
