using UserManagement.Domain.Exceptions;

namespace UserManagement.Domain.Entities;

public sealed class Role
{
    public const int NameMaxLength = 50;

    private readonly List<Permission> _permissions = [];

    public Role(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > NameMaxLength)
            throw new DomainException("El nombre del rol no es válido.");

        Id = id;
        Name = name;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    public void AddPermission(Permission permission)
    {
        ArgumentNullException.ThrowIfNull(permission);

        if (_permissions.Any(p => p.Id == permission.Id))
            return;

        _permissions.Add(permission);
    }
}