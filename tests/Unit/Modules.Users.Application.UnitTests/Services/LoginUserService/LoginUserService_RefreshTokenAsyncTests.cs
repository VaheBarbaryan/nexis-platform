using System.Security.Authentication;
using FluentAssertions;
using Modules.Users.Application.Contracts;
using Modules.Users.Application.Options;
using Modules.Users.Domain;
using Modules.Users.Domain.Roles.ValueObjects;
using Modules.Users.Domain.Users;
using Modules.Users.Domain.Users.Repositories;
using Modules.Users.Domain.Users.ValueObjects;
using Moq;

namespace Modules.Users.Application.UnitTests.Services.LoginUserService;

public class LoginUserService_RefreshTokenAsyncTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IJwtProvider> _jwtProvider = new();
    private readonly Mock<ITokenStore<Guid>> _refreshTokenStore = new();
    private readonly Mock<ITokenGenerator> _tokenGenerator = new();
    private readonly Mock<IUserUnitOfWork> _unitOfWork = new();

    private readonly Microsoft.Extensions.Options.IOptions<JwtOptions> _jwtOptions =
        Microsoft.Extensions.Options.Options.Create(new JwtOptions
        {
            RefreshTokenExpirationDays = 7
        });

    private ILoginUserService CreateService()
        => new Application.Services.LoginUserService(
            _userRepository.Object,
            _passwordHasher.Object,
            _jwtProvider.Object,
            _refreshTokenStore.Object,
            _tokenGenerator.Object,
            _unitOfWork.Object,
            _jwtOptions);

    [Fact]
    public async Task RefreshTokenAsync_Should_Throw_When_Refresh_Token_Is_Invalid_Or_Expired()
    {
        // Arrange
        var service = CreateService();
        const string refreshToken = "refreshToken";
        const string hashedRefreshToken = "hashedRefreshToken";

        _tokenGenerator.Setup(x => x.Hash(refreshToken)).Returns(hashedRefreshToken);
        _refreshTokenStore.Setup(x => x.GetDeleteAsync(hashedRefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty);

        // Act
        var act = () => service.RefreshTokenAsync(refreshToken);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialException>();
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_Throw_When_User_Not_Found()
    {
        // Arrange
        var service = CreateService();
        const string refreshToken = "refreshToken";
        const string hashedRefreshToken = "hashedRefreshToken";
        var userId = Guid.NewGuid();

        _tokenGenerator.Setup(x => x.Hash(refreshToken)).Returns(hashedRefreshToken);
        _refreshTokenStore.Setup(x => x.GetDeleteAsync(hashedRefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);
        _userRepository.Setup(x => x.GetByIdAsync(new UserId(userId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => service.RefreshTokenAsync(refreshToken);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialException>();
    }


    [Fact]
    public async Task RefreshTokenAsync_Should_Throw_If_Token_Reused()
    {
        // Arrange
        var service = CreateService();
        const string refreshToken = "refreshToken";
        const string hashedRefreshToken = "hashedRefreshToken";

        var userId = Guid.NewGuid();

        _tokenGenerator.Setup(x => x.Hash(refreshToken)).Returns(hashedRefreshToken);

        _refreshTokenStore.SetupSequence(x => x.GetDeleteAsync(hashedRefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId) // first call: token exists -> valid
            .ReturnsAsync(Guid.Empty); // second call: token already used -> null

        // Act: first refresh should succeed
        _refreshTokenStore.Setup(x => x.RemoveAsync(hashedRefreshToken, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _userRepository.Setup(x => x.GetByIdAsync(new UserId(userId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(User.Create(
                new UserId(userId),
                Email.Create("test@gmail.com"),
                Username.Create("username"),
                Password.Create("hashed_password"),
                new RoleId(Guid.NewGuid())
            ));
        _jwtProvider.Setup(x => x.GenerateAccessToken(It.IsAny<User>())).Returns("newAccessToken");
        _jwtProvider.Setup(x => x.GenerateRefreshToken(It.IsAny<User>())).Returns("newRefreshToken");

        await service.RefreshTokenAsync(refreshToken);

        // Act: second refresh (reuse) should throw
        var act = () => service.RefreshTokenAsync(refreshToken);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialException>("Token reuse should be detected");
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_Return_Login_Result()
    {
        // Arrange
        var service = CreateService();
        const string refreshToken = "refreshToken";
        const string hashedRefreshToken = "hashedRefreshToken";
        const string newHashedRefreshToken = "newHashedRefreshToken";
        const string newAccessToken = "newAccessToken";
        const string newRefreshToken = "newRefreshToken";
        var userId = Guid.NewGuid();
        var user = User.Create(
            new UserId(userId),
            Email.Create("test@gmail.com"),
            Username.Create("username"),
            Password.Create("hashed_password"),
            new RoleId(Guid.NewGuid()));

        _tokenGenerator.Setup(x => x.Hash(It.IsAny<string>()))
            .Returns<string>(token => token == refreshToken ? hashedRefreshToken : newHashedRefreshToken);
        _refreshTokenStore.Setup(x => x.GetDeleteAsync(hashedRefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);
        _refreshTokenStore.Setup(x => x.RemoveAsync(hashedRefreshToken, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _refreshTokenStore.Setup(x =>
            x.StoreAsync(
                user.Id.Value,
                newHashedRefreshToken,
                TimeSpan.FromDays(_jwtOptions.Value.RefreshTokenExpirationDays),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _userRepository.Setup(x => x.GetByIdAsync(new UserId(userId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _jwtProvider.Setup(x => x.GenerateAccessToken(user)).Returns(newAccessToken);
        _jwtProvider.Setup(x => x.GenerateRefreshToken(user)).Returns(newRefreshToken);

        // Act
        var result = await service.RefreshTokenAsync(refreshToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<LoginResult>();
        result.AccessToken.Should().Be(newAccessToken);
        result.RefreshToken.Should().Be(newRefreshToken);

        _tokenGenerator.Verify(x => x.Hash(refreshToken), Times.Once);
        _tokenGenerator.Verify(x => x.Hash(newRefreshToken), Times.Once);
        _refreshTokenStore.Verify(x => x.RemoveAsync(hashedRefreshToken, It.IsAny<CancellationToken>()), Times.Once);
        _refreshTokenStore.Verify(x =>
            x.StoreAsync(user.Id.Value, newHashedRefreshToken, It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }
}
