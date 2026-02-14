using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Application.Contracts;
using Modules.Users.Domain.Users.Repositories;
using Modules.Users.Domain.Users.ValueObjects;
using SharedKernel.Infrastructure;

namespace Modules.Users.Application.Services;

public class PasswordService : IPasswordService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ITokenStore<Guid> _passwordTokenStore;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public PasswordService(
        IUserRepository userRepository,
        ITokenGenerator tokenGenerator,
        [FromKeyedServices(TokenStoreKey.PasswordReset)]
        ITokenStore<Guid> passwordTokenStore,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _passwordTokenStore = passwordTokenStore;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task ForgotPasswordAsync(string email, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, ct);

        if (user?.EmailVerifiedAt is not null)
        {
            user.RequestPasswordReset();
            await _unitOfWork.CommitAsync(ct);
        }

        await Task.Delay(150, ct);
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken ct = default)
    {
        Console.WriteLine($"Token: {token}");
        var tokenHash = _tokenGenerator.Hash(token);

        Guid? userId = await _passwordTokenStore.GetAsync(tokenHash, ct);
        Console.WriteLine($"User ID: {userId.Value}");

        if (!userId.HasValue)
        {
            return false;
        }

        var user = await _userRepository.GetByIdAsync(new UserId(userId.Value), ct);

        Console.WriteLine($"User: {user}");

        if (user is null || _passwordHasher.Verify(newPassword, user.Password.Value))
        {
            return false;
        }

        await _passwordTokenStore.RemoveAsync(tokenHash, ct);

        user.ChangePassword(_passwordHasher.Hash(newPassword));
        await _unitOfWork.CommitAsync(ct);

        return true;
    }
}
