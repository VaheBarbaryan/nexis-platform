namespace Modules.Users.Application.Contracts;

public interface IVerificationLinkBuilder
{
    Uri Build(string token);
}
