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
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
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
}
