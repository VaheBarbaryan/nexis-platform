using Modules.Users.Domain.Users.ValueObjects;

namespace Modules.Users.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId userId, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(Email email, CancellationToken ct = default);

    Task<bool> UsernameExistsAsync(Username username, CancellationToken ct = default);

    void Add(User user);
}
