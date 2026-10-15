using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Constants;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public const int AdministratorId = 1;
    public const int ReadOnlyId = 2;

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();
        builder.Property(r => r.Name).HasMaxLength(Role.NameMaxLength).IsRequired();
        builder.HasIndex(r => r.Name).IsUnique();

        builder.HasData(
            new Role(AdministratorId, RoleNames.Administrator),
            new Role(ReadOnlyId, RoleNames.ReadOnly));

        builder.HasMany(r => r.Permissions)
            .WithMany()
            .UsingEntity(
                "RolePermissions",
                right => right.HasOne(typeof(Permission)).WithMany().HasForeignKey("PermissionId"),
                left => left.HasOne(typeof(Role)).WithMany().HasForeignKey("RoleId"),
                join =>
                {
                    join.HasKey("RoleId", "PermissionId");
                    join.HasData(
                        new { RoleId = AdministratorId, PermissionId = PermissionConfiguration.UsersReadId },
                        new { RoleId = AdministratorId, PermissionId = PermissionConfiguration.UsersUpdateId },
                        new { RoleId = ReadOnlyId, PermissionId = PermissionConfiguration.UsersReadId });
                });
    }
}