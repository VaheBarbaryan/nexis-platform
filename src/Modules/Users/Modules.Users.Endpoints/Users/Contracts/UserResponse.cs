namespace Modules.Users.Endpoints.Users.Contracts;

public sealed record UserResponse(
    string Id,
    string? Email,
    string? Username);
