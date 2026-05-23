using FluentAssertions;
using Modules.Users.Application.Contracts;
using Modules.Users.Domain;
using Modules.Users.Domain.Roles.ValueObjects;
using Modules.Users.Domain.Users;
using Modules.Users.Domain.Users.Repositories;
using Modules.Users.Domain.Users.ValueObjects;
using Moq;

namespace Modules.Users.Application.UnitTests.Services.PasswordService;

public class PasswordService_ResetPasswordAsyncTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<ITokenGenerator> _tokenGenerator = new();
    private readonly Mock<ITokenStore<Guid>> _passwordTokenStore = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IUserUnitOfWork> _unitOfWork = new();

    private IPasswordService CreateService()
        => new Application.Services.PasswordService(
            _userRepository.Object,
            _tokenGenerator.Object,
            _passwordTokenStore.Object,
            _passwordHasher.Object,
            _unitOfWork.Object);

    [Fact]
    public async Task ResetPasswordAsync_Should_Not_Change_Password_When_Token_Is_Not_Valid()
    {
        // Arrange
        var service = CreateService();
        const string token = "token";
        const string hashedToken = "hashed_token";
        const string newPassword = "new_password";

        _tokenGenerator.Setup(x => x.Hash(token)).Returns(hashedToken);
        _passwordTokenStore.Setup(x => x.GetAsync(hashedToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid()); // no user in repo -> returns false

        // Act
        var result = await service.ResetPasswordAsync(token, newPassword);

        // Assert
        result.Should().BeFalse();

        _passwordHasher.Verify(x => x.Hash(newPassword), Times.Never);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Not_Change_Password_When_User_Not_Found()
    {
        // Arrange
        var service = CreateService();
        const string token = "token";
        const string hashedToken = "hashed_token";
        const string newPassword = "new_password";
        var userId = Guid.NewGuid();

        _tokenGenerator.Setup(x => x.Hash(token)).Returns(hashedToken);
        _passwordTokenStore.Setup(x => x.GetAsync(hashedToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);
        _userRepository.Setup(x => x.GetByIdAsync(UserId.From(userId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await service.ResetPasswordAsync(token, newPassword);

        // Assert
        result.Should().BeFalse();

        _passwordHasher.Verify(x => x.Hash(newPassword), Times.Never);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Not_Change_Password_When_Password_Is_Same()
    {
        // Arrange
        var service = CreateService();
        const string token = "token";
        const string hashedToken = "hashed_token";
        const string initialPassword = "initial_password";
        const string newPassword = "new_password";
        var user = User.Create(
            Email.From("test@gmail.com"),
            Username.From("testuser"),
            Password.From(initialPassword),
            RoleId.New()
        );

        _tokenGenerator.Setup(x => x.Hash(token)).Returns(hashedToken);
        _passwordTokenStore.Setup(x => x.GetAsync(hashedToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user.Id.Value);
        _userRepository.Setup(x => x.GetByIdAsync(UserId.From(user.Id.Value), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(x => x.Verify(newPassword, initialPassword)).Returns(true);

        // Act
        var result = await service.ResetPasswordAsync(token, newPassword);

        // Assert
        result.Should().BeFalse();

        _passwordHasher.Verify(x => x.Hash(newPassword), Times.Never);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Change_Password_And_Return_True()
    {
        // Arrange
        var service = CreateService();
        const string token = "token";
        const string hashedToken = "hashed_token";
        const string initialPassword = "initial_password";
        const string newPassword = "new_password";
        const string newPasswordHash = "new_password_hash";
        var user = User.Create(
            Email.From("test@gmail.com"),
            Username.From("testuser"),
            Password.From(initialPassword),
            RoleId.New()
        );

        _tokenGenerator.Setup(x => x.Hash(token)).Returns(hashedToken);
        _passwordTokenStore.Setup(x => x.GetAsync(hashedToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user.Id.Value);
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(x => x.Hash(newPassword)).Returns(newPasswordHash);
        _passwordTokenStore.Setup(x => x.RemoveAsync(hashedToken, It.IsAny<CancellationToken>()));

        // Act
        var result = await service.ResetPasswordAsync(token, newPassword);

        // Assert
        result.Should().BeTrue();

        user.Password.Value.Should().Be(newPasswordHash);
        user.Password.Value.Should().NotBe(initialPassword);

        _passwordHasher.Verify(x => x.Hash(newPassword), Times.Once);
        _passwordTokenStore.Verify(x => x.RemoveAsync(hashedToken, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
