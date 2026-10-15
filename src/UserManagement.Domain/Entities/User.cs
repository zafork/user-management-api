using UserManagement.Domain.Exceptions;

namespace UserManagement.Domain.Entities;

public sealed class User
{
    public const int EmailMaxLength = 256;
    public const int FullNameMaxLength = 150;

    private readonly List<Role> _roles = [];

    private User() { }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    public static User Create(string email, string fullName, string passwordHash, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("El hash de la contraseña es obligatorio.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            PasswordHash = passwordHash,
            IsActive = true,
            CreatedAt = createdAt
        };
        user.SetEmail(email);
        user.SetFullName(fullName);
        return user;
    }

    public void Update(string fullName, string email, bool isActive, DateTime updatedAt)
    {
        SetFullName(fullName);
        SetEmail(email);
        IsActive = isActive;
        UpdatedAt = updatedAt;
    }

    public void AssignRole(Role role)
    {
        ArgumentNullException.ThrowIfNull(role);

        if (_roles.Any(r => r.Id == role.Id))
            return;

        _roles.Add(role);
    }

    public IReadOnlyCollection<string> GetPermissions() =>
        _roles.SelectMany(r => r.Permissions)
              .Select(p => p.Code)
              .Distinct()
              .ToList();

    public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("El email es obligatorio.");

        var normalized = NormalizeEmail(email);
        if (normalized.Length > EmailMaxLength || !normalized.Contains('@'))
            throw new DomainException("El email no tiene un formato válido.");

        Email = normalized;
    }

    private void SetFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("El nombre es obligatorio.");

        var trimmed = fullName.Trim();
        if (trimmed.Length > FullNameMaxLength)
            throw new DomainException($"El nombre no puede superar {FullNameMaxLength} caracteres.");

        FullName = trimmed;
    }
}