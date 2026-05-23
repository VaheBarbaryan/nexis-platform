using Modules.Users.Domain.Roles.ValueObjects;
using Modules.Users.Domain.Users.Events;
using Modules.Users.Domain.Users.Exceptions;
using Modules.Users.Domain.Users.Rules;
using Modules.Users.Domain.Users.ValueObjects;
using SharedKernel.Domain.Aggregates;

namespace Modules.Users.Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
    private readonly List<UserRole> _roles = new();
    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

    public Email Email { get; private set; } = null!;
    public Username Username { get; private set; } = null!;
    public Password Password { get; private set; } = null!;
    public DateTimeOffset? EmailVerifiedAt { get; private set; }
    public Bio? Bio { get; private set; }
    public Location? Location { get; private set; }
    public Website? Website { get; private set; }
    public BirthDate? BirthDate { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private User()
    {
    }

    private User(Email email, Username username, Password password)
    {
        Id = UserId.New();
        Email = email;
        Username = username;
        Password = password;

        var now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public static User Create(
        Email email,
        Username username,
        Password password,
        RoleId defaultRoleId)
    {
        var user = new User(email, username, password);

        user.AssignRole(defaultRoleId);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(
            user.Id.Value,
            user.Username.Value,
            user.Email.Value
        ));

        user.RaiseDomainEvent(new EmailVerificationRequestedDomainEvent(
            user.Id.Value,
            user.Username.Value,
            user.Email.Value
        ));

        return user;
    }

    public void UpdateProfile(Bio? bio, Location? location, Website? website, BirthDate? birthDate)
    {
        Bio = bio;
        Location = location;
        Website = website;
        BirthDate = birthDate;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AssignRole(RoleId roleId)
    {
        CheckRule(new UserCannotHaveDuplicateRoleRule(_roles, roleId));
        _roles.Add(new UserRole(Id, roleId));
    }

    public void MarkEmailVerified()
    {
        CheckRule(new UserEmailMustNotBeAlreadyVerifiedRule(EmailVerifiedAt));

        EmailVerifiedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new UserEmailVerifiedDomainEvent(
            Id.Value,
            Username.Value,
            Email.Value
        ));
    }

    public void ChangePassword(string password)
    {
        CheckRule(new NewPasswordMustBeDifferentFromCurrentRule(password, Password.Value));

        Password = Password.From(password);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void EnsureEmailIsVerified()
    {
        if (EmailVerifiedAt is null)
        {
            throw new EmailNotVerifiedException();
        }
    }

    public void RequestEmailVerification()
    {
        if (EmailVerifiedAt is not null) return;

        RaiseDomainEvent(new EmailVerificationRequestedDomainEvent(
            Id.Value,
            Username.Value,
            Email.Value
        ));
    }

    public void RequestPasswordReset()
    {
        RaiseDomainEvent(new PasswordResetRequestedDomainEvent(
            Id.Value,
            Username.Value,
            Email.Value
        ));
    }
}
