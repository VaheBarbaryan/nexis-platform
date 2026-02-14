namespace Modules.Users.Endpoints.Users.Contracts;

public sealed record ResetPasswordRequest(
    string Token,
    string NewPassword,
    string NewPasswordConfirm);
