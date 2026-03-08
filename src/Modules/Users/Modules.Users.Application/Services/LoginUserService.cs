using System.Security.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modules.Users.Application.Contracts;
using Modules.Users.Application.Options;
using Modules.Users.Domain;
using Modules.Users.Domain.Users.Repositories;
using Modules.Users.Domain.Users.ValueObjects;

namespace Modules.Users.Application.Services;

public sealed class LoginUserService : ILoginUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly ITokenStore<Guid> _refreshTokenStore;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUserUnitOfWork _unitOfWork;

    private readonly JwtOptions _jwtOptions;

    public LoginUserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        [FromKeyedServices(TokenStoreKey.RefreshToken)]
        ITokenStore<Guid> refreshTokenStore,
        ITokenGenerator tokenGenerator,
        IUserUnitOfWork unitOfWork,
        IOptions<JwtOptions> jwtOptions)
    {
        ArgumentNullException.ThrowIfNull(jwtOptions);

        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _refreshTokenStore = refreshTokenStore;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<LoginResult> LoginAsync(
        string email,
        string password,
        CancellationToken ct = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, ct);

        if (user is null)
        {
            throw new InvalidCredentialException("Invalid email or password.");
        }

        user.EnsureEmailIsVerified();

        if (!_passwordHasher.Verify(password, user.Password.Value))
        {
            throw new InvalidCredentialException("Invalid email or password.");
        }

        if (_passwordHasher.NeedsRehash(user.Password.Value))
        {
            user.ChangePassword(_passwordHasher.Hash(password));
            await _unitOfWork.CommitAsync(ct);
        }

        var accessToken = _jwtProvider.GenerateAccessToken(user);
        var refreshToken = _jwtProvider.GenerateRefreshToken(user);

        var hashedRefreshToken = _tokenGenerator.Hash(refreshToken);
        await _refreshTokenStore.StoreAsync(
            user.Id.Value,
            hashedRefreshToken,
            TimeSpan.FromDays(_jwtOptions.RefreshTokenExpirationDays),
            ct);

        return new LoginResult(accessToken, refreshToken);
    }

    public async Task<LoginResult> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var hashedRefreshToken = _tokenGenerator.Hash(refreshToken);

        Guid? userId = await _refreshTokenStore.GetDeleteAsync(hashedRefreshToken, ct);

        if (!userId.HasValue)
        {
            throw new InvalidCredentialException("Invalid refresh token.");
        }

        var user = await _userRepository.GetByIdAsync(new UserId(userId.Value), ct);

        if (user is null)
        {
            throw new InvalidCredentialException("Invalid refresh token.");
        }

        await _refreshTokenStore.RemoveAsync(hashedRefreshToken, ct);

        var newAccessToken = _jwtProvider.GenerateAccessToken(user);
        var newRefreshToken = _jwtProvider.GenerateRefreshToken(user);

        var hashedNewRefreshToken = _tokenGenerator.Hash(newRefreshToken);
        await _refreshTokenStore.StoreAsync(
            user.Id.Value,
            hashedNewRefreshToken,
            TimeSpan.FromDays(_jwtOptions.RefreshTokenExpirationDays),
            ct);

        return new LoginResult(newAccessToken, newRefreshToken);
    }
}
