namespace Modules.Users.Endpoints.Users.Contracts;

public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string NewPasswordConfirm);
