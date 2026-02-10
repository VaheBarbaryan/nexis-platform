using Modules.Users.Application.Contracts;
using Modules.Users.Domain.Users.Repositories;
using SharedKernel.Infrastructure;

namespace Modules.Users.Application.Services;

public class PasswordService : IPasswordService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PasswordService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
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
}
