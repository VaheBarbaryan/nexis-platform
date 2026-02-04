using Modules.Users.Application.Contracts;
using Modules.Users.Application.Security;
using Modules.Users.Domain.Roles.Exceptions;
using Modules.Users.Domain.Roles.Repositories;
using Modules.Users.Domain.Users;
using Modules.Users.Domain.Users.Exceptions;
using Modules.Users.Domain.Users.Repositories;
using Modules.Users.Domain.Users.ValueObjects;
using SharedKernel.Infrastructure;

namespace Modules.Users.Application.Services;

public sealed class RegisterUserService : IRegisterUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IEmailVerificationTokenStore _emailVerificationTokenStore;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IEmailVerificationTokenStore emailVerificationTokenStore,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _emailVerificationTokenStore = emailVerificationTokenStore;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> RegisterAsync(
        string email,
        string username,
        string password,
        CancellationToken ct = default)
    {
        var emailVo = Email.Create(email);
        var usernameVo = Username.Create(username);

        if (await _userRepository.EmailExistsAsync(emailVo, ct))
        {
            throw new EmailAlreadyExistsException();
        }

        if (await _userRepository.UsernameExistsAsync(usernameVo, ct))
        {
            throw new InvalidUsernameException("User with this username already exists.");
        }

        var hash = _passwordHasher.Hash(password);
        var passwordVo = Password.Create(hash);

        var role = await _roleRepository.GetByNameAsync(SystemRoles.User);

        if (role is null)
        {
            throw new RoleNotFoundException();
        }

        var userId = new UserId(Guid.NewGuid());
        var user = User.Create(userId, emailVo, usernameVo, passwordVo, role.Id);

        _userRepository.Add(user);

        await _unitOfWork.CommitAsync(ct);

        return user.Id.Value;
    }

    public async Task<User> VerifyEmailAsync(string token, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Verification token missing.");
        }

        string tokenHash = _tokenGenerator.Hash(token);
        Guid? userId = await _emailVerificationTokenStore.GetAsync(tokenHash, ct);

        if (userId is null)
        {
            throw new InvalidOperationException("Token is invalid.");
        }

        var userIdVo = new UserId(userId.Value);
        var user = await _userRepository.GetByIdAsync(userIdVo, ct);

        if (user is null)
        {
            throw new UserNotFoundException("User not found.");
        }

        if (user.EmailVerifiedAt is not null)
        {
            return user;
        }

        user.MarkEmailVerified();
        await _unitOfWork.CommitAsync(ct);

        return user;
    }
}
