namespace Modules.Users.Application.Contracts;

public interface IEmailVerificationTokenStore
{
    Task StoreAsync(
        Guid userId,
        string tokenHash,
        TimeSpan ttl,
        CancellationToken ct);

    Task<Guid?> GetAsync(
        string tokenHash,
        CancellationToken ct);

    Task RemoveAsync(
        string tokenHash,
        CancellationToken ct);
}
