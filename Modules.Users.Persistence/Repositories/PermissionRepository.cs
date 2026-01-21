using Microsoft.EntityFrameworkCore;
using Modules.Users.Application.Contracts;
using Modules.Users.Domain.Permissions;
using Modules.Users.Domain.Permissions.ValueObjects;
using Modules.Users.Persistence.Contexts;

namespace Modules.Users.Persistence.Repositories;

public sealed class PermissionRepository : IPermissionRepository
{
    private readonly UsersDbContext _context;

    public PermissionRepository(UsersDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(PermissionName permissionName)
    {
        return await _context.Permissions
            .AnyAsync(p => p.Name == permissionName);
    }

    public async Task AddAsync(Permission permission)
    {
        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Permission> permissions)
    {
        _context.Permissions.AddRange(permissions);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Permission>> GetAllAsync()
    {
        return await _context.Permissions.ToListAsync();
    }

    public async Task<Dictionary<PermissionName, Permission>> GetAllAsDictionary()
    {
        return await _context.Permissions.ToDictionaryAsync(p => p.Name, p => p);
    }
}
