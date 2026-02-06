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
    public Username Username { get; private set; }  = null!;
    public Password Password { get; private set; }  = null!;
    public DateTimeOffset? EmailVerifiedAt { get; private set; }
    public string? Bio { get; private set; }
    public string? Location { get; private set; }
    public string? Website { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private User() {}

    private User(UserId id, Email email, Username username, Password password)
    {
        Id = id;
        Email = email;
        Username = username;
        Password = password;

        var now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public static User Create(
        UserId userId,
        Email email,
        Username username,
        Password password,
        RoleId defaultRoleId,
        DateTime? emailVerifiedAt = null,
        string? bio = null,
        string? location = null,
        string? website = null,
        DateTime? birthDate = null)
    {
        var user = new User(userId, email, username, password)
        {
            EmailVerifiedAt = emailVerifiedAt,
            Bio = bio,
            Location = location,
            Website = website,
            BirthDate = birthDate
        };

        user.AssignRole(defaultRoleId);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(
            user.Id.Value,
            user.Username.Value,
            user.Email.Value
        ));

        return user;
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
    }

    public void ChangePassword(string password)
    {
        Password = Password.Create(password);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void EnsureEmailIsVerified()
    {
        if (EmailVerifiedAt is null)
        {
            throw new EmailNotVerifiedException();
        }
    }
}
