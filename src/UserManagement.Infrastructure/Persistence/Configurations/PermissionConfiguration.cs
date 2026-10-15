using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Constants;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Persistence.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public const int UsersReadId = 1;
    public const int UsersUpdateId = 2;

    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.Code).HasMaxLength(Permission.CodeMaxLength).IsRequired();
        builder.HasIndex(p => p.Code).IsUnique();

        builder.HasData(
            new Permission(UsersReadId, PermissionCodes.UsersRead),
            new Permission(UsersUpdateId, PermissionCodes.UsersUpdate));
    }
}