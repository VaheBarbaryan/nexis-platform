namespace Modules.Users.Application.Contracts;

public interface IEmailVerificationTokenStore
{
    Task StoreAsync(
        Guid userId,
        string tokenHash,
        TimeSpan ttl,
        CancellationToken ct);

    Task<string?> GetAsync(
        Guid userId,
        CancellationToken ct);

    Task RemoveAsync(
        Guid userId,
        CancellationToken ct);
}
