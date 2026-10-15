namespace UserManagement.Domain.Constants;

public static class PermissionCodes
{
    public const string UsersRead = "users.read";
    public const string UsersUpdate = "users.update";

    public static readonly IReadOnlyList<string> All = [UsersRead, UsersUpdate];
}