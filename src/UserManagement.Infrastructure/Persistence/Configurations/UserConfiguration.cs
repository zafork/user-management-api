using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public static readonly Guid AdminUserId = new("ba435598-dded-4609-a1fa-47760ebd17a5");
    public static readonly Guid ReadOnlyUserId = new("c65c123f-10e1-4abe-b96a-cad13d8c790c");
    public static readonly Guid InactiveUserId = new("6eeb52d3-15a8-4c2e-9850-0afdeab1bef2");

    private static readonly DateTime SeedCreatedAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.Email).HasMaxLength(User.EmailMaxLength).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.FullName).HasMaxLength(User.FullNameMaxLength).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(100).IsRequired();
        builder.Property(u => u.IsActive).IsRequired();
        builder.Property(u => u.CreatedAt).IsRequired();

        builder.HasData(
            new
            {
                Id = AdminUserId,
                Email = "admin@demo.com",
                FullName = "Administrador",
                PasswordHash = "$2a$12$VklczTU7tSV9kXvo/xhTv.Xf62B9c05upBoW3Y2XVItFDyTRK78e6",
                IsActive = true,
                CreatedAt = SeedCreatedAt
            },
            new
            {
                Id = ReadOnlyUserId,
                Email = "consulta@demo.com",
                FullName = "Usuario Consulta",
                PasswordHash = "$2a$12$8V0lKPmSAA/v2VonpQmgwea0/NuEsE2yXJusEWmYsP87mVgromJX.",
                IsActive = true,
                CreatedAt = SeedCreatedAt
            },
            new
            {
                Id = InactiveUserId,
                Email = "inactivo@demo.com",
                FullName = "Usuario Inactivo",
                PasswordHash = "$2a$12$AyQZJv17xMcLJk2TNpNODe1V6e2EX3CfEglm06v4AyLt9FqHolYTK",
                IsActive = false,
                CreatedAt = SeedCreatedAt
            });

        builder.HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity(
                "UserRoles",
                right => right.HasOne(typeof(Role)).WithMany().HasForeignKey("RoleId"),
                left => left.HasOne(typeof(User)).WithMany().HasForeignKey("UserId"),
                join =>
                {
                    join.HasKey("UserId", "RoleId");
                    join.HasData(
                        new { UserId = AdminUserId, RoleId = RoleConfiguration.AdministratorId },
                        new { UserId = ReadOnlyUserId, RoleId = RoleConfiguration.ReadOnlyId },
                        new { UserId = InactiveUserId, RoleId = RoleConfiguration.ReadOnlyId });
                });
    }
}