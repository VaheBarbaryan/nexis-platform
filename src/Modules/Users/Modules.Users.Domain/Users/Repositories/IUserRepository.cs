using Modules.Users.Domain.Users.ValueObjects;

namespace Modules.Users.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId userId, CancellationToken ct = default);

    Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default);

    Task<bool> EmailExistsAsync(Email email, CancellationToken ct = default);

    Task<bool> UsernameExistsAsync(Username username, CancellationToken ct = default);

    void Add(User user);
}
