using Modules.Users.Application.Contracts;
using Modules.Users.Application.Security;
using Modules.Users.Domain.Users;
using Modules.Users.Domain.Users.Exceptions;
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
        CancellationToken cancellationToken = default)
    {
        var emailVo = Email.Create(email);
        var usernameVo = Username.Create(username);
        
        if (await _userRepository.EmailExistsAsync(emailVo, cancellationToken))
        {
            throw new EmailAlreadyExistsException();
        }

        if (await _userRepository.UsernameExistsAsync(usernameVo, cancellationToken))
        {
            throw new InvalidUsernameException();
        }
        
        var hash = _passwordHasher.Hash(password);
        var passwordVo = Password.Create(hash);

        var role = await _roleRepository.GetByNameAsync(SystemRoles.User);

        if (role is null)
        {
            throw new ApplicationException("Role not found");
        }
        
        var user = User.Create(emailVo, usernameVo, passwordVo, role.Id);
        
        _userRepository.Add(user);
        
        await _unitOfWork.CommitAsync(cancellationToken);
        
        return user.Id.Value;
    }
}