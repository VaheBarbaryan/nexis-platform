using Modules.Users.Application.Security;
using Modules.Users.Domain.Permissions;
using Modules.Users.Domain.Permissions.Repositories;
using Modules.Users.Domain.Permissions.ValueObjects;
using Modules.Users.Domain.Roles;
using Modules.Users.Domain.Roles.Repositories;
using Modules.Users.Domain.Roles.ValueObjects;
using SharedKernel.Application;

namespace Modules.Users.Application.Seed;

public sealed class RolePermissionSeeder : IModuleSeeder
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public RolePermissionSeeder(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task SeedAsync()
    {
        await SeedPermissionsAsync();

        var permissionMap = await _permissionRepository.GetAllAsDictionary();

        await CreateRoleIfMissing(SystemRoles.User, [
            PermissionCatalog.TweetCreate,
            PermissionCatalog.TweetLike,
            PermissionCatalog.ProfileEdit
        ], permissionMap);

        await CreateRoleIfMissing(SystemRoles.Moderator, [
            PermissionCatalog.TweetDelete,
            PermissionCatalog.UserMute
        ], permissionMap);

        await CreateRoleIfMissing(SystemRoles.Admin, [
            PermissionCatalog.TweetDelete,
            PermissionCatalog.UserMute,
            PermissionCatalog.UserDelete,
            PermissionCatalog.UserBan,
            PermissionCatalog.PimAccess
        ], permissionMap);
    }

    public async Task SeedPermissionsAsync()
    {
        var existing = await _permissionRepository.GetAllAsync();

        var permissionsToSeed = new[]
        {
            PermissionCatalog.TweetCreate,
            PermissionCatalog.TweetLike,
            PermissionCatalog.TweetDelete,
            PermissionCatalog.ProfileEdit,
            PermissionCatalog.UserMute,
            PermissionCatalog.UserBan,
            PermissionCatalog.UserDelete,
            PermissionCatalog.PimAccess
        };

        foreach (var permName in permissionsToSeed)
        {
            if (!existing.Any(p => p.Name == permName))
            {
                var permission = Permission.Create(PermissionName.From(permName.Value));
                await _permissionRepository.AddAsync(permission);
            }
        }
    }


    private async Task CreateRoleIfMissing(
        RoleName roleName,
        IEnumerable<PermissionName> codes,
        Dictionary<PermissionName, Permission> permissionMap)
    {
        if (await _roleRepository.ExistsAsync(roleName))
            return;

        var role = Role.Create(roleName);

        foreach (var code in codes)
        {
            if (!permissionMap.TryGetValue(code, out var permission))
            {
                throw new InvalidOperationException($"Permission '{code}' is missing in the DB.");
            }

            role.AddPermission(permission.Id);
        }

        await _roleRepository.AddAsync(role);
    }
}
