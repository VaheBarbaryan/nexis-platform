using Microsoft.EntityFrameworkCore;
using Modules.Users.Application.Contracts;
using Modules.Users.Domain.Roles;
using Modules.Users.Domain.Roles.Repositories;
using Modules.Users.Domain.Roles.ValueObjects;
using Modules.Users.Persistence.Contexts;

namespace Modules.Users.Persistence.Repositories;

public sealed class RoleRepository : IRoleRepository
{
    private readonly UsersDbContext _context;

    public RoleRepository(UsersDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(RoleName roleName)
    {
        return await _context.Roles
            .AnyAsync(r => r.Name == roleName);
    }

    public async Task AddAsync(Role role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
    }

    public async Task AddIfNotExistsAsync(Role role)
    {
        ArgumentNullException.ThrowIfNull(role);

        if (await ExistsAsync(role.Name))
            return;

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
    }

    public async Task<Role?> GetByNameAsync(RoleName roleName)
    {
        return await _context.Roles
            .Include(r => r.Permissions) // backing field
            .FirstOrDefaultAsync(r => r.Name == roleName);
    }
}
