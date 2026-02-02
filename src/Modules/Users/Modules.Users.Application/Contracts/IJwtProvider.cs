using Modules.Users.Domain.Users;

namespace Modules.Users.Application.Contracts;

public interface IJwtProvider
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(User user);
}