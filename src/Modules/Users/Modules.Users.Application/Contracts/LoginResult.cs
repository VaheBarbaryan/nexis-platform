namespace Modules.Users.Application.Contracts;

public sealed record LoginResult(string AccessToken, string RefreshToken);
