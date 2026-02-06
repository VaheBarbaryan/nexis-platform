using System.Security.Authentication;
using Microsoft.Extensions.Options;
using Modules.Users.Application.Contracts;
using Modules.Users.Application.Options;
using Modules.Users.Domain.Users.Repositories;
using SharedKernel.Infrastructure;

namespace Modules.Users.Application.Services;

public sealed class LoginUserService : ILoginUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    private readonly JwtOptions _jwtOptions;

    public LoginUserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        IRefreshTokenStore refreshTokenStore,
        ITokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork,
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
            user.Id.Value.ToString(),
            hashedRefreshToken,
            TimeSpan.FromDays(_jwtOptions.RefreshTokenExpirationDays),
            ct);

        return new LoginResult(accessToken, refreshToken);
    }
}
