using Modules.Users.Domain.Roles.ValueObjects;
using Modules.Users.Domain.Users.Events;
using Modules.Users.Domain.Users.Rules;
using Modules.Users.Domain.Users.ValueObjects;
using SharedKernel.Domain.Aggregates;

namespace Modules.Users.Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
    private readonly List<UserRole> _roles = new();

    public IReadOnlyCollection<UserRole> Roles => _roles;
    
    public Email Email { get; private set; }
    public string Username { get; private set; }
    public string? Bio { get; private set; }
    public string? Location { get; private set; }
    public string? Website { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    private User() {}

    private User(UserId id, Email email, string username)
    {
        Id = id;
        Email = email;
        Username = username;
        
        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public static User Create(
        UserId id, 
        Email email, 
        string username,
        RoleId defaultRoleId,
        string? bio = null,
        string? location = null,
        string? website = null,
        DateTime? birthDate = null)
    {
        var user = new User(id, email, username)
        {
            Bio = bio,
            Location = location,
            Website = website,
            BirthDate = birthDate
        };
        
        user.AssignRole(defaultRoleId);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(
            user.Id.Value,
            user.Username,
            user.Email.Value
        ));
        
        return user;
    }
    
    public void AssignRole(RoleId roleId)
    {
        CheckRule(new UserCannotHaveDuplicateRoleRule(_roles, roleId));
        _roles.Add(new UserRole(Id, roleId));
    }
}