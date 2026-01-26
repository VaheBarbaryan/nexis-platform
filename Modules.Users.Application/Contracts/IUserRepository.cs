using Modules.Users.Domain.Users;
using Modules.Users.Domain.Users.ValueObjects;

namespace Modules.Users.Application.Contracts;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(Email email, CancellationToken ct = default);
    
    Task<bool> UsernameExistsAsync(Username username, CancellationToken ct = default);

    void Add(User user);
}