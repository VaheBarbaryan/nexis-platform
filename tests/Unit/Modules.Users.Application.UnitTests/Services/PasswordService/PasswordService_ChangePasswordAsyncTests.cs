using FluentAssertions;
using Modules.Users.Application.Contracts;
using Modules.Users.Domain.Roles.ValueObjects;
using Modules.Users.Domain.Users;
using Modules.Users.Domain.Users.Exceptions;
using Modules.Users.Domain.Users.Repositories;
using Modules.Users.Domain.Users.ValueObjects;
using Moq;
using SharedKernel.Infrastructure;

namespace Modules.Users.Application.UnitTests.Services.PasswordService;

public class PasswordService_ChangePasswordAsyncTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<ITokenGenerator> _tokenGenerator = new();
    private readonly Mock<ITokenStore<Guid>> _passwordTokenStore = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private IPasswordService CreateService()
        => new Application.Services.PasswordService(
            _userRepository.Object,
            _tokenGenerator.Object,
            _passwordTokenStore.Object,
            _passwordHasher.Object,
            _unitOfWork.Object);

    [Fact]
    public async Task ChangePasswordAsync_Should_Throw_When_User_Not_Found()
    {
        // Arrange
        var service = CreateService();
        var userId = Guid.NewGuid();
        var userIdVo = new UserId(userId);

        _userRepository.Setup(x => x.GetByIdAsync(userIdVo, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => service.ChangePasswordAsync(userId, "initialPassword", "newPassword");

        // Assert
        await act.Should().ThrowAsync<UserNotFoundException>();

        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_Should_Throw_When_New_Password_Is_The_Same_As_Old_Password()
    {
        // Arrange
        var service = CreateService();
        var userId = Guid.NewGuid();
        var userIdVo = new UserId(userId);
        var initialPassword = "my_password";
        var user = CreateTestUser(userId, initialPassword);

        _userRepository.Setup(x => x.GetByIdAsync(userIdVo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => user);
        _passwordHasher.Setup(x => x.Verify(initialPassword, user.Password.Value)).Returns(true);

        // Act
        var act = () => service.ChangePasswordAsync(userId, initialPassword, initialPassword);

        // Assert
        await act.Should().ThrowAsync<PasswordReuseException>();

        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_Should_Throw_When_Current_Password_Is_Invalid()
    {
        // Arrange
        var service = CreateService();
        var userId = Guid.NewGuid();
        var userIdVo = new UserId(userId);
        var initialPassword = "my_password";
        var user = CreateTestUser(userId, initialPassword);

#pragma warning disable CA1861
        var passwordHasherResultsQueue = new Queue<bool>(new[] { true, false });
#pragma warning restore CA1861

        _userRepository.Setup(x => x.GetByIdAsync(userIdVo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => user);
        _passwordHasher.Setup(x => x.Verify(initialPassword, user.Password.Value))
            .Returns(() => passwordHasherResultsQueue.Dequeue());

        // Act
        var act = () => service.ChangePasswordAsync(userId, "some_password", "new_password");

        // Assert
        await act.Should().ThrowAsync<IncorrectPasswordException>();
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_Should_Update_Password()
    {
        // Arrange
        var service = CreateService();
        var userId = Guid.NewGuid();
        var userIdVo = new UserId(userId);
        var initialPassword = "my_password";
        var newPassword = "new_password";
        var newPasswordHash = "new_password_hash";
        var user = CreateTestUser(userId, initialPassword);

        _userRepository.Setup(x => x.GetByIdAsync(userIdVo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => user);
        _passwordHasher.Setup(x => x.Verify(newPassword, user.Password.Value))
            .Returns(false);
        _passwordHasher.Setup(x => x.Verify(initialPassword, user.Password.Value))
            .Returns(true);
        _passwordHasher.Setup(x => x.Hash(newPassword)).Returns(newPasswordHash);

        // Act
        await service.ChangePasswordAsync(userId, initialPassword, newPassword);

        // Assert
        user.Password.Value.Should().Be(newPasswordHash);
        user.Password.Value.Should().NotBe(initialPassword);

        _passwordHasher.Verify(x => x.Hash(newPassword), Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static User CreateTestUser(Guid id, string password = "password") =>
        User.Create(
            new UserId(id),
            Email.Create("test@gmail.com"),
            Username.Create("username"),
            Password.Create(password),
            new RoleId(Guid.NewGuid()));
}
