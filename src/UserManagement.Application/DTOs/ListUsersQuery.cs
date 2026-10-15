using System.ComponentModel.DataAnnotations;

namespace UserManagement.Application.DTOs;

public sealed record ListUsersQuery
{
    public const int MaxPageSize = 100;
    public const int MaxPage = int.MaxValue / MaxPageSize;

    [Range(1, MaxPage)]
    public int Page { get; init; } = 1;

    [Range(1, MaxPageSize)]
    public int PageSize { get; init; } = 20;
}