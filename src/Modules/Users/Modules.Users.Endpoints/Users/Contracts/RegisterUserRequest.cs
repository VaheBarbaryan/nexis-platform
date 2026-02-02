namespace Modules.Users.Endpoints.Users.Contracts;

public sealed record RegisterUserRequest(
    string Username,
    string Email, 
    string Password, 
    string PasswordConfirm);