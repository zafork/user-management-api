using UserManagement.Domain.Exceptions;

namespace UserManagement.Domain.Entities;

public sealed class Permission
{
    public const int CodeMaxLength = 100;

    public Permission(int id, string code)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length > CodeMaxLength)
            throw new DomainException("El código del permiso no es válido.");

        Id = id;
        Code = code;
    }

    public int Id { get; private set; }
    public string Code { get; private set; }
}