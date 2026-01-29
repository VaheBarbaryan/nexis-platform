using Modules.Users.Domain.Roles.ValueObjects;

namespace Modules.Users.Domain.Roles.Repositories;

public interface IRoleRepository
{
    Task<bool> ExistsAsync(RoleName roleName);

    Task AddAsync(Role role);

    Task AddIfNotExistsAsync(Role role);

    Task<Role?> GetByNameAsync(RoleName roleName);
}
