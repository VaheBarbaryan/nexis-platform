namespace Modules.Users.Application.Contracts;

public interface IPasswordResetLinkBuilder
{
    Uri Build(string token);
}
