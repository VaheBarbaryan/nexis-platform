namespace Modules.Users.Application.Contracts;

public interface ITokenGenerator
{
    string Generate(int size = 32);
    string Hash(string token);
    bool Verify(string token, string hash);
}
