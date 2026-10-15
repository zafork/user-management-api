using System.ComponentModel.DataAnnotations;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.DTOs;

public sealed record UpdateUserRequest
{
    [Required, MaxLength(User.FullNameMaxLength)]
    public string FullName { get; init; } = string.Empty;

    [Required, EmailAddress, MaxLength(User.EmailMaxLength)]
    public string Email { get; init; } = string.Empty;

    [Required]
    public bool? IsActive { get; init; }
}