using Modules.Users.Domain.Permissions;
using Modules.Users.Domain.Permissions.ValueObjects;

namespace Modules.Users.Application.Contracts;

public interface IPermissionRepository
{
    Task<bool> ExistsAsync(PermissionName permissionName);

    Task AddAsync(Permission permission);

    Task AddRangeAsync(IEnumerable<Permission> permissions);

    Task<IReadOnlyList<Permission>> GetAllAsync();

    Task<Dictionary<PermissionName, Permission>> GetAllAsDictionary();
}