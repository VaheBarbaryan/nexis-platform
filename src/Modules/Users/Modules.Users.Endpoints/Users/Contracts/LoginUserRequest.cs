namespace Modules.Users.Endpoints.Users.Contracts;

public sealed record LoginUserRequest(
    string Email,
    string Password);
