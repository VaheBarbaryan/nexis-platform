using Modules.Users.Application.Contracts;
using Modules.Users.Domain;
using Modules.Users.Domain.Roles.ValueObjects;
using Modules.Users.Domain.Users;
using Modules.Users.Domain.Users.Repositories;
using Modules.Users.Domain.Users.ValueObjects;
using Moq;

namespace Modules.Users.Application.UnitTests.Services.PasswordService;

public class PasswordService_ForgotPasswordAsyncTests
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
    public async Task ForgotPasswordAsync_Should_Not_Request_Password_Reset_When_User_Not_Found()
    {
        // Arrange
        var service = CreateService();
        var email = "test@gmail.com";

        _userRepository.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        await service.ForgotPasswordAsync(email);

        // Assert
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ForgotPasswordAsync_Should_Not_Request_Password_Reset_When_User_Email_Is_Not_Verified()
    {
        // Arrange
        var service = CreateService();
        var email = "test@gmail.com";
        var user = CreateUser(email);

        _userRepository.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await service.ForgotPasswordAsync(email);

        // Assert
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ForgotPasswordAsync_Should_Have_Consistent_Timing_To_Prevent_Timing_Attacks()
    {
        // Arrange
        var service = CreateService();
        var email = "test@gmail.com";

        _userRepository.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await service.ForgotPasswordAsync(email);
        stopwatch.Stop();

        // Assert - should take at least 100ms due to delay
        Assert.True(stopwatch.ElapsedMilliseconds >= 100);
    }

    [Fact]
    public async Task ForgotPasswordAsync_Should_Commit_When_User_Exists_And_Email_Verified()
    {
        // Arrange
        var service = CreateService();
        var email = "test@gmail.com";
        var user = CreateUser(email);

        user.MarkEmailVerified();

        _userRepository.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await service.ForgotPasswordAsync(email);

        // Assert
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static User CreateUser(string email)
    {
        return User.Create(
            new UserId(Guid.NewGuid()),
            Email.Create(email),
            Username.Create("testuser"),
            Password.Create("Password123!"),
            new RoleId(Guid.NewGuid())
        );
    }
}
