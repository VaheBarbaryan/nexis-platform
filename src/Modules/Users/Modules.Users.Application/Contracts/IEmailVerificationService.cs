using Modules.Users.Domain.Users;

namespace Modules.Users.Application.Contracts;

public interface IEmailVerificationService
{
    Task<User> VerifyEmailAsync(string token, CancellationToken ct = default);

    Task ResendEmailVerificationAsync(string email, CancellationToken ct = default);
}
