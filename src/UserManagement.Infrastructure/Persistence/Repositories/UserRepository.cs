using Microsoft.EntityFrameworkCore;
using UserManagement.Application.DTOs;
using UserManagement.Application.Ports.Out;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Users
          .Include(u => u.Roles)
          .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        db.Users
          .Include(u => u.Roles)
          .ThenInclude(r => r.Permissions)
          .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = db.Users.Where(u => u.Email == email);

        if (excludeUserId is not null)
            query = query.Where(u => u.Id != excludeUserId.Value);

        return query.AnyAsync(cancellationToken);
    }

    public async Task<PagedResult<User>> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await db.Users.CountAsync(cancellationToken);

        var items = await db.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .OrderBy(u => u.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<User>(items, page, pageSize, totalCount);
    }
}