namespace SharedKernel.Application.Auth;

public interface ICurrentUser
{
    Guid? Id { get; }

    string? Email { get; }

    string? Username { get; }
}
