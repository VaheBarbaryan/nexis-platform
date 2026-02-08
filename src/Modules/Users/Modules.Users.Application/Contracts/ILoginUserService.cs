namespace Modules.Users.Application.Contracts;

public interface ILoginUserService
{
    Task<LoginResult> LoginAsync(string email, string password, CancellationToken ct = default);

    Task<LoginResult> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}
