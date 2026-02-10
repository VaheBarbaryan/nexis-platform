namespace Modules.Users.Application.Contracts;

public interface IPasswordService
{
    Task ForgotPasswordAsync(string email, CancellationToken ct = default);
}
