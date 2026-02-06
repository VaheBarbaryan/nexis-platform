using System.Security.Authentication;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Modules.Users.Application.Contracts;
using Modules.Users.Application.Options;
using Modules.Users.Domain.Roles.ValueObjects;
using Modules.Users.Domain.Users;
using Modules.Users.Domain.Users.Exceptions;
using Modules.Users.Domain.Users.Repositories;
using Modules.Users.Domain.Users.ValueObjects;
using Moq;
using SharedKernel.Infrastructure;

namespace Modules.Users.Application.UnitTests.Services.LoginUserService;

public class LoginUserService_LoginAsyncTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IJwtProvider> _jwtProvider = new();
    private readonly Mock<IRefreshTokenStore> _refreshTokenStore = new();
    private readonly Mock<ITokenGenerator> _tokenGenerator = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

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
    public async Task LoginAsync_Should_Throw_When_User_With_Email_Not_Found()
    {
        // Arrange
        var service = CreateService();

        _userRepository
            .Setup(x => x.GetByEmailAsync("test@gmail.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => service.LoginAsync("test@gmail.com", It.IsAny<string>());

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialException>();
    }

    [Fact]
    public async Task LoginAsync_Should_Throw_When_User_Has_Not_Verified_Email()
    {
        // Arrange
        var service = CreateService();
        var user = User.Create(
            new UserId(Guid.NewGuid()),
            Email.Create("test@gmail.com"),
            Username.Create("user"),
            Password.Create("hashed_password"),
            new RoleId(Guid.NewGuid()));

        _userRepository
            .Setup(x => x.GetByEmailAsync("test@gmail.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var act = () => service.LoginAsync("test@gmail.com", "123456789");

        // Assert
        await act.Should().ThrowAsync<EmailNotVerifiedException>();
    }

    [Fact]
    public async Task LoginAsync_Should_Throw_When_Password_Is_Invalid()
    {
        // Arrange
        var service = CreateService();
        var user = User.Create(
            new UserId(Guid.NewGuid()),
            Email.Create("test@gmail.com"),
            Username.Create("username"),
            Password.Create("hashed_password"),
            new RoleId(Guid.NewGuid()));

        user.MarkEmailVerified();

        _userRepository
            .Setup(x => x.GetByEmailAsync("test@gmail.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher
            .Setup(x => x.Verify("123456789", "hashed_password"))
            .Returns(false);

        // Act
        var act = () => service.LoginAsync("test@gmail.com", "123456789");

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialException>();
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Login_Result_Without_Password_Rehash()
    {
        // Arrange
        var service = CreateService();
        var user = User.Create(
            new UserId(Guid.NewGuid()),
            Email.Create("test@gmail.com"),
            Username.Create("username"),
            Password.Create("hashed_password"),
            new RoleId(Guid.NewGuid()));

        user.MarkEmailVerified();

        _userRepository
            .Setup(x => x.GetByEmailAsync("test@gmail.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher
            .Setup(x => x.Verify("123456789", "hashed_password"))
            .Returns(true);
        _passwordHasher.Setup(x => x.NeedsRehash("hashed_password")).Returns(false);
        _jwtProvider.Setup(x => x.GenerateAccessToken(user)).Returns("access_token");
        _jwtProvider.Setup(x => x.GenerateRefreshToken(user)).Returns("refresh_token");
        _tokenGenerator.Setup(x => x.Hash("refresh_token")).Returns("hashed_refresh_token");
        _refreshTokenStore
            .Setup(x =>
                x.StoreAsync(
                    user.Id.Value.ToString(),
                    "hashed_refresh_token",
                    TimeSpan.FromDays(_jwtOptions.Value.RefreshTokenExpirationDays),
                    It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        // Act
        var result = await service.LoginAsync("test@gmail.com", "123456789");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<LoginResult>();
        result.AccessToken.Should().Be("access_token");
        result.RefreshToken.Should().Be("refresh_token");

        _refreshTokenStore.Verify(
            x => x.StoreAsync(
                user.Id.Value.ToString(),
                "hashed_refresh_token",
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _passwordHasher.Verify(x => x.Hash(It.IsAny<string>()), Times.Never);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Login_Result_With_Password_Rehash()
    {
        // Arrange
        var service = CreateService();
        var user = User.Create(
            new UserId(Guid.NewGuid()),
            Email.Create("test@gmail.com"),
            Username.Create("username"),
            Password.Create("hashed_password"),
            new RoleId(Guid.NewGuid()));

        user.MarkEmailVerified();

        _userRepository
            .Setup(x => x.GetByEmailAsync("test@gmail.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher
            .Setup(x => x.Verify("123456789", "hashed_password"))
            .Returns(true);
        _passwordHasher.Setup(x => x.NeedsRehash("hashed_password")).Returns(true);
        _passwordHasher.Setup(x => x.Hash("123456789")).Returns("new_hashed_password");
        _jwtProvider.Setup(x => x.GenerateAccessToken(user)).Returns("access_token");
        _jwtProvider.Setup(x => x.GenerateRefreshToken(user)).Returns("refresh_token");
        _tokenGenerator.Setup(x => x.Hash("refresh_token")).Returns("hashed_refresh_token");
        _refreshTokenStore
            .Setup(x =>
                x.StoreAsync(
                    user.Id.Value.ToString(),
                    "hashed_refresh_token",
                    TimeSpan.FromDays(_jwtOptions.Value.RefreshTokenExpirationDays),
                    It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        // Act
        var result = await service.LoginAsync("test@gmail.com", "123456789");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<LoginResult>();
        result.AccessToken.Should().Be("access_token");
        result.RefreshToken.Should().Be("refresh_token");
        user.Password.Value.Should().Be("new_hashed_password");

        _refreshTokenStore.Verify(
            x => x.StoreAsync(
                user.Id.Value.ToString(),
                "hashed_refresh_token",
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
