namespace Modules.Users.Application.Contracts;

public interface ITokenGenerator
{
    string Generate();
    string Hash(string token);
    bool Verify(string token, string hash);
}
