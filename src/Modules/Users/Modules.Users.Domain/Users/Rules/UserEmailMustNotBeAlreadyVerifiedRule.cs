using SharedKernel.Domain.Rules;

namespace Modules.Users.Domain.Users.Rules;

public sealed class UserEmailMustNotBeAlreadyVerifiedRule : IBusinessRule
{
    private readonly bool _isVerified;

    public UserEmailMustNotBeAlreadyVerifiedRule(DateTimeOffset? emailVerifiedAt)
    {
        _isVerified = emailVerifiedAt is not null;
    }

    public bool IsBroken() => _isVerified;

    public string Message => "User email is already verified.";
}
