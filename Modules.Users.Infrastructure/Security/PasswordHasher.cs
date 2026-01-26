using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Modules.Users.Application.Contracts;

namespace Modules.Users.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 24;
    private const int HashSize = 32;
    private const int Iterations = 4;
    private const int MemoryKb = 65536; // 64MB
    private const int Parallelism = 2;

    private const string AlgorithmTag = "ARGON2ID";

    private readonly byte[] _pepper;

    public PasswordHasher(string pepper)
    {
        if (string.IsNullOrWhiteSpace(pepper))
        {
            throw new ArgumentException("Pepper must be provided", nameof(pepper));
        }

        _pepper = Encoding.UTF8.GetBytes(pepper);
    }

    // =========================
    // PUBLIC API
    // =========================

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var pepperedPassword = Combine(passwordBytes, _pepper);

        using var argon2 = CreateArgon2(pepperedPassword, salt, Iterations, MemoryKb, Parallelism);

        var hash = argon2.GetBytes(HashSize);

        CryptographicOperations.ZeroMemory(passwordBytes);
        CryptographicOperations.ZeroMemory(pepperedPassword);

        return FormatHash(salt, hash);
    }

    public bool Verify(string password, string storedHash)
    {
        if (!TryParseHash(storedHash, out var parameters))
            return false;

        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var pepperedPassword = Combine(passwordBytes, _pepper);

        using var argon2 = CreateArgon2(
            pepperedPassword,
            parameters.Salt,
            parameters.Iterations,
            parameters.MemoryKb,
            parameters.Parallelism);

        var computedHash = argon2.GetBytes(parameters.Hash.Length);

        CryptographicOperations.ZeroMemory(passwordBytes);
        CryptographicOperations.ZeroMemory(pepperedPassword);

        return CryptographicOperations.FixedTimeEquals(
            parameters.Hash,
            computedHash);
    }

    public bool NeedsRehash(string storedHash)
    {
        if (!TryParseHash(storedHash, out var parameters))
            return true;

        if (parameters.Iterations < Iterations)
            return true;

        if (parameters.MemoryKb < MemoryKb)
            return true;

        if (parameters.Parallelism < Parallelism)
            return true;

        return false;
    }

    // =========================
    // INTERNALS
    // =========================

    private static Argon2id CreateArgon2(
        byte[] password,
        byte[] salt,
        int iterations,
        int memoryKb,
        int parallelism)
    {
        return new Argon2id(password)
        {
            Salt = salt,
            Iterations = iterations,
            MemorySize = memoryKb,
            DegreeOfParallelism = parallelism
        };
    }

    private static byte[] Combine(byte[] first, byte[] second)
    {
        var result = new byte[first.Length + second.Length];

        Buffer.BlockCopy(first, 0, result, 0, first.Length);
        Buffer.BlockCopy(second, 0, result, first.Length, second.Length);

        return result;
    }

    private static string FormatHash(byte[] salt, byte[] hash)
    {
        return string.Join('$',
            AlgorithmTag,
            Iterations,
            MemoryKb,
            Parallelism,
            Convert.ToHexString(salt),
            Convert.ToHexString(hash));
    }

    private static bool TryParseHash(
        string storedHash,
        out HashParameters parameters)
    {
        parameters = default!;

        var parts = storedHash.Split('$');

        if (parts.Length != 6)
            return false;

        if (parts[0] != AlgorithmTag)
            return false;

        try
        {
            parameters = new HashParameters
            {
                Iterations = int.Parse(parts[1]),
                MemoryKb = int.Parse(parts[2]),
                Parallelism = int.Parse(parts[3]),
                Salt = Convert.FromHexString(parts[4]),
                Hash = Convert.FromHexString(parts[5])
            };

            return true;
        }
        catch
        {
            return false;
        }
    }

    private sealed class HashParameters
    {
        public required int Iterations { get; init; }
        public required int MemoryKb { get; init; }
        public required int Parallelism { get; init; }
        public required byte[] Salt { get; init; }
        public required byte[] Hash { get; init; }
    }
}
