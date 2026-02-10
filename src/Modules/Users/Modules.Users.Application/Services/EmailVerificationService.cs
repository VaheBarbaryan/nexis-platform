using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Application.Contracts;
using Modules.Users.Domain.Users;
using Modules.Users.Domain.Users.Exceptions;
using Modules.Users.Domain.Users.Repositories;
using Modules.Users.Domain.Users.ValueObjects;
using SharedKernel.Infrastructure;

namespace Modules.Users.Application.Services;

public class EmailVerificationService : IEmailVerificationService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ITokenStore<Guid> _emailVerificationTokenStore;

    public EmailVerificationService(
        IUserRepository userRepository,
        ITokenGenerator tokenGenerator,
        [FromKeyedServices(TokenStoreKey.EmailVerification)]
        ITokenStore<Guid> emailVerificationTokenStore,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _emailVerificationTokenStore = emailVerificationTokenStore;
        _unitOfWork = unitOfWork;
    }

    public async Task<User> VerifyEmailAsync(string token, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Verification token missing.");
        }

        string tokenHash = _tokenGenerator.Hash(token);
        Guid? userId = await _emailVerificationTokenStore.GetDeleteAsync(tokenHash, ct);

        if (!userId.HasValue || userId.Value == Guid.Empty)
        {
            throw new InvalidOperationException("Token is invalid.");
        }

        var userIdVo = new UserId(userId.Value);
        var user = await _userRepository.GetByIdAsync(userIdVo, ct);

        if (user is null)
        {
            throw new UserNotFoundException();
        }

        if (user.EmailVerifiedAt is not null)
        {
            return user;
        }

        user.MarkEmailVerified();
        await _unitOfWork.CommitAsync(ct);

        return user;
    }

    public async Task ResendEmailVerificationAsync(string email, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, ct);

        if (user is not null && user.EmailVerifiedAt is null)
        {
            user.RequestEmailVerification();
        }

        await _unitOfWork.CommitAsync(ct);
    }
}
