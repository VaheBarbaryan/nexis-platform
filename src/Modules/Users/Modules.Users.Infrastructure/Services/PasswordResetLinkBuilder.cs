using Microsoft.Extensions.Options;
using Modules.Users.Application.Contracts;
using Modules.Users.Infrastructure.Configuration;

namespace Modules.Users.Infrastructure.Services;

public class PasswordResetLinkBuilder : IPasswordResetLinkBuilder
{
    private readonly string _frontendBaseUrl;

    public PasswordResetLinkBuilder(IOptions<FrontendOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _frontendBaseUrl = options.Value.BaseUrl;
    }

    public Uri Build(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be empty", nameof(token));

        var url = $"{_frontendBaseUrl.TrimEnd('/')}/password-reset?token={Uri.EscapeDataString(token)}";
        return new Uri(url);
    }
}
