namespace Modules.Users.Application.Contracts;

public interface IRefreshTokenStore
{
    Task StoreAsync(
        string userId,
        string refreshTokenHash,
        TimeSpan expiresIn,
        CancellationToken ct = default);

    Task<Guid?> GetAsync(string refreshTokenHash, CancellationToken ct = default);
}
