using SharedKernel.Domain.Rules;

namespace Modules.Users.Domain.Users.Rules;

public sealed class NewPasswordMustBeDifferentFromCurrentRule : IBusinessRule
{
    private readonly string _newPassword;
    private readonly string _oldPassword;

    internal NewPasswordMustBeDifferentFromCurrentRule(string newPassword, string oldPassword)
    {
        _oldPassword = oldPassword;
        _newPassword = newPassword;
    }

    public bool IsBroken() => _oldPassword == _newPassword;

    public string Message => "New password must be different from your current password";
}
