using Modules.Users.Application.Contracts;
using Modules.Users.Domain;
using Modules.Users.Domain.Roles.ValueObjects;
using Modules.Users.Domain.Users;
using Modules.Users.Domain.Users.Repositories;
using Modules.Users.Domain.Users.ValueObjects;
using Moq;

namespace Modules.Users.Application.UnitTests.Services.EmailVerificationService;

public class EmailVerificationService_ResendEmailVerificationAsyncTests
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

    [Fact]
    public async Task ResendEmailVerificationAsync_Should_Commit_Successfully_When_User_Is_Unverified()
    {
        // Arrange
        var email = Email.From("test@gmail.com");
        var user = User.Create(
            email,
            Username.From("testuser"),
            Password.From("Password123!"),
            RoleId.New()
        );

        _userRepository
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var service = CreateService();

        // Act
        var exception = await Record.ExceptionAsync(() =>
            service.ResendEmailVerificationAsync("test@gmail.com"));

        // Assert
        Assert.Null(exception); // method completes successfully
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResendEmailVerificationAsync_Should_Commit_Successfully_When_User_Does_Not_Exist()
    {
        _userRepository
            .Setup(r => r.GetByEmailAsync(Email.From("missing@gmail.com"), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var service = CreateService();

        var exception = await Record.ExceptionAsync(() =>
            service.ResendEmailVerificationAsync("missing@gmail.com"));

        Assert.Null(exception);
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResendEmailVerificationAsync_Should_Commit_Successfully_User_Is_Already_Verified()
    {
        var email = Email.From("verified@gmail.com");
        var user = User.Create(
            email,
            Username.From("verifieduser"),
            Password.From("Password123!"),
            RoleId.New()
        );

        user.MarkEmailVerified();

        _userRepository
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var service = CreateService();

        var exception = await Record.ExceptionAsync(() =>
            service.ResendEmailVerificationAsync("verified@gmail.com"));

        Assert.Null(exception);
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
