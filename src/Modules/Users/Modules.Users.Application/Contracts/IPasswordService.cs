namespace Modules.Users.Application.Contracts;

public interface IPasswordService
{
    Task ForgotPasswordAsync(string email, CancellationToken ct = default);

    Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken ct = default);

    Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken ct = default);
}
