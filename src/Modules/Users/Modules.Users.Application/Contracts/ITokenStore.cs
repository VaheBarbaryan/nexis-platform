namespace Modules.Users.Application.Contracts;

public interface ITokenStore<TIdentifier>
{
    Task StoreAsync(
        TIdentifier identifier,
        string tokenHash,
        TimeSpan ttl,
        CancellationToken ct = default);

    Task<TIdentifier?> GetAsync(
        string tokenHash,
        CancellationToken ct = default);

    Task<TIdentifier?> GetDeleteAsync(
        string tokenHash,
        CancellationToken ct = default);

    Task RemoveAsync(
        string tokenHash,
        CancellationToken ct = default);
}
