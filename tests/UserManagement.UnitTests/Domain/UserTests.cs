using UserManagement.Domain.Entities;
using UserManagement.Domain.Exceptions;

namespace UserManagement.UnitTests.Domain;

public sealed class UserTests
{
    private static readonly DateTime Now = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_normaliza_el_email_y_deja_el_usuario_activo()
    {
        var user = User.Create("  Ana@Demo.COM ", "Ana", "hash", Now);

        Assert.Equal("ana@demo.com", user.Email);
        Assert.True(user.IsActive);
        Assert.NotEqual(Guid.Empty, user.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("sin-arroba")]
    public void Create_con_email_invalido_lanza_DomainException(string email)
    {
        Assert.Throws<DomainException>(() => User.Create(email, "Ana", "hash", Now));
    }

    [Fact]
    public void GetPermissions_devuelve_los_permisos_de_todos_los_roles_sin_repetir()
    {
        var read = new Permission(1, "users.read");
        var update = new Permission(2, "users.update");
        var admin = new Role(1, "Administrador");
        admin.AddPermission(read);
        admin.AddPermission(update);
        var viewer = new Role(2, "Consulta");
        viewer.AddPermission(read);

        var user = User.Create("ana@demo.com", "Ana", "hash", Now);
        user.AssignRole(admin);
        user.AssignRole(viewer);

        Assert.Equal(["users.read", "users.update"], user.GetPermissions().Order());
    }

    [Fact]
    public void Update_cambia_los_datos_y_registra_la_fecha()
    {
        var user = User.Create("ana@demo.com", "Ana", "hash", Now);
        var later = Now.AddHours(1);

        user.Update("Ana María", "ana.maria@demo.com", isActive: false, later);

        Assert.Equal("Ana María", user.FullName);
        Assert.Equal("ana.maria@demo.com", user.Email);
        Assert.False(user.IsActive);
        Assert.Equal(later, user.UpdatedAt);
    }
}