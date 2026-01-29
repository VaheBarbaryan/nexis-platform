using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using Modules.Users.Application.Contracts;
using Modules.Users.Infrastructure.Configuration;

namespace Modules.Users.Infrastructure.Services;

[SuppressMessage(
    "Design",
    "CA1054:URI-like parameters should not be strings",
    Justification = "Integration event contracts use language-neutral primitives")]
public sealed class VerificationLinkBuilder : IVerificationLinkBuilder
{
    private readonly string _frontendBaseUrl;

    public VerificationLinkBuilder(IOptions<FrontendOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _frontendBaseUrl = options.Value.BaseUrl;
    }

    public Uri Build(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be empty", nameof(token));

        var url = $"{_frontendBaseUrl.TrimEnd('/')}/verify-email?token={Uri.EscapeDataString(token)}";
        return new Uri(url);
    }
}
