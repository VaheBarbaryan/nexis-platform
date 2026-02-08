using Modules.Users.Domain.Users;

namespace Modules.Users.Application.Contracts;

public interface IRegisterUserService
{
    Task<Guid> RegisterAsync(string email, string username, string password, CancellationToken ct = default);
}
