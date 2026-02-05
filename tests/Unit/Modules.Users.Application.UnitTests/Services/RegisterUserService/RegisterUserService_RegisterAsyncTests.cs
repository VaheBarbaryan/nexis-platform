using FluentAssertions;
using Modules.Users.Application.Contracts;
using Modules.Users.Application.Security;
using Modules.Users.Domain.Roles;
using Modules.Users.Domain.Roles.Exceptions;
using Modules.Users.Domain.Roles.Repositories;
using Modules.Users.Domain.Roles.ValueObjects;
using Modules.Users.Domain.Users;
using Modules.Users.Domain.Users.Exceptions;
using Modules.Users.Domain.Users.Repositories;
using Modules.Users.Domain.Users.ValueObjects;
using Moq;
using SharedKernel.Infrastructure;

namespace Modules.Users.Application.UnitTests.Services.RegisterUserService;

public class RegisterUserService_RegisterAsyncTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IRoleRepository> _roleRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<ITokenGenerator> _tokenGenerator = new();
    private readonly Mock<IEmailVerificationTokenStore> _tokenStore = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private IRegisterUserService CreateService()
        => new Application.Services.RegisterUserService(
            _userRepository.Object,
            _roleRepository.Object,
            _passwordHasher.Object,
            _tokenGenerator.Object,
            _tokenStore.Object,
            _unitOfWork.Object);

    [Fact]
    public async Task RegisterAsync_Should_Throw_When_Email_Already_Exists()
    {
        // Arrange
        var service = CreateService();

        _userRepository
            .Setup(x => x.EmailExistsAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = () => service.RegisterAsync(
            "test@gmail.com",
            "username",
            "password",
            It.IsAny<CancellationToken>());

        // Assert
        await act.Should().ThrowAsync<EmailAlreadyExistsException>();
    }

    [Fact]
    public async Task RegisterAsync_Should_Throw_When_Username_Already_Exists()
    {
        // Arrange
        var service = CreateService();

        _userRepository
            .Setup(x => x.EmailExistsAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _userRepository
            .Setup(x => x.UsernameExistsAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = () => service.RegisterAsync(
            "test@gmail.com",
            "username",
            "password",
            It.IsAny<CancellationToken>());

        // Assert
        await act.Should().ThrowAsync<InvalidUsernameException>();
    }

    [Fact]
    public async Task RegisterAsync_Should_Throw_When_Role_Not_Found()
    {
        // Arrange
        var service = CreateService();
        var password = "strong_password";

        _userRepository
            .Setup(x => x.EmailExistsAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _userRepository
            .Setup(x => x.UsernameExistsAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _passwordHasher.Setup(x => x.Hash(password)).Returns("hashed_password");
        _roleRepository
            .Setup(x => x.GetByNameAsync(It.IsAny<RoleName>()))
            .ReturnsAsync((Role?)null);

        // Act
        var act = () => service.RegisterAsync(
            "test@gmail.com",
            "username",
            password,
            It.IsAny<CancellationToken>());

        // Assert
        await act.Should().ThrowAsync<RoleNotFoundException>();
    }

    [Fact]
    public async Task RegisterAsync_Should_Create_User_With_Correct_Details_And_Commit()
    {
        // Arrange
        var service = CreateService();
        var username = "username";
        var email = "test@gmail.com";
        var password = "strong_password";
        var hashedPassword = "hashed_password";
        var role = Role.Create(SystemRoles.User.Value);

        User? capturedUser = null;

        _userRepository
            .Setup(x => x.EmailExistsAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _userRepository
            .Setup(x => x.UsernameExistsAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _userRepository
            .Setup(x => x.Add(It.IsAny<User>()))
            .Callback<User>(user => capturedUser = user);
        _passwordHasher.Setup(x => x.Hash(password)).Returns(hashedPassword);
        _roleRepository
            .Setup(x => x.GetByNameAsync(It.Is<RoleName>(r => r == SystemRoles.User)))
            .ReturnsAsync(role);

        // Act
        var result = await service.RegisterAsync(
            email,
            username,
            password,
            CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();

        _userRepository.Verify(x => x.Add(It.IsAny<User>()), Times.Once);
        capturedUser.Should().NotBeNull();
        capturedUser!.Id.Value.Should().Be(result);
        capturedUser.Email.Value.Should().Be(email);
        capturedUser.Username.Value.Should().Be(username);
        capturedUser.Password.Value.Should().Be(hashedPassword);
        capturedUser.Roles.Should().Contain(userRole => userRole.RoleId == role.Id);

        _passwordHasher.Verify(x => x.Hash(password), Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
