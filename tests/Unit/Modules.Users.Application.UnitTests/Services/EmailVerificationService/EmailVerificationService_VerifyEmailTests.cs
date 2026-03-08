using FluentAssertions;
using Modules.Users.Application.Contracts;
using Modules.Users.Domain;
using Modules.Users.Domain.Roles.ValueObjects;
using Modules.Users.Domain.Users;
using Modules.Users.Domain.Users.Exceptions;
using Modules.Users.Domain.Users.Repositories;
using Modules.Users.Domain.Users.ValueObjects;
using Moq;

namespace Modules.Users.Application.UnitTests.Services.EmailVerificationService;

public class EmailVerificationService_VerifyEmailTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<ITokenGenerator> _tokenGenerator = new();
    private readonly Mock<ITokenStore<Guid>> _tokenStore = new();
    private readonly Mock<IUserUnitOfWork> _unitOfWork = new();

    private Application.Services.EmailVerificationService CreateService()
        => new(
            _userRepository.Object,
            _tokenGenerator.Object,
            _tokenStore.Object,
            _unitOfWork.Object);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task VerifyEmail_Should_Throw_When_Token_Is_Null_Or_Empty(string? token)
    {
        // Arrange
        var service = CreateService();

        // Act
        var act = () => service.VerifyEmailAsync(token!);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task VerifyEmailAsync_Should_Throw_When_Token_Is_Invalid()
    {
        // Arrange
        var service = CreateService();

        _tokenGenerator.Setup(x => x.Hash("token")).Returns("hashed_token");
        _tokenStore
            .Setup(x => x.GetDeleteAsync("hashed_token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty);

        // Act
        var act = () => service.VerifyEmailAsync("token");

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task VerifyEmailAsync_Should_Throw_When_User_Not_Found()
    {
        // Arrange
        var service = CreateService();
        var userId = Guid.NewGuid();

        _tokenGenerator.Setup(x => x.Hash("token")).Returns("hashed_token");
        _tokenStore
            .Setup(x => x.GetDeleteAsync("hashed_token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);
        _userRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => service.VerifyEmailAsync("token");

        // Assert
        await act.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    public async Task VerifyEmailAsync_Should_Return_User_When_Already_Verified()
    {
        // Arrange
        var service = CreateService();
        var userId = Guid.NewGuid();

        var user = User.Create(
            new UserId(userId),
            Email.Create("test@mail.com"),
            Username.Create("user"),
            Password.Create("hashed_password"),
            new RoleId(Guid.NewGuid()));

        user.MarkEmailVerified();

        _tokenGenerator.Setup(x => x.Hash("token")).Returns("hash");
        _tokenStore.Setup(x => x.GetDeleteAsync("hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        _userRepository.Setup(x =>
                x.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await service.VerifyEmailAsync("token");

        // Assert
        result.Should().Be(user);

        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task VerifyEmailAsync_Should_Mark_Email_Verified_And_Commit()
    {
        // Arrange
        var service = CreateService();
        var userId = Guid.NewGuid();

        var user = User.Create(
            new UserId(userId),
            Email.Create("test@mail.com"),
            Username.Create("user"),
            Password.Create("hashed_password"),
            new RoleId(Guid.NewGuid()));

        _tokenGenerator.Setup(x => x.Hash("token")).Returns("hash");
        _tokenStore
            .Setup(x => x.GetDeleteAsync("hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        _userRepository.Setup(x =>
                x.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await service.VerifyEmailAsync("token");

        // Assert
        result.Should().Be(user);
        result.EmailVerifiedAt.Should().NotBeNull();

        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
