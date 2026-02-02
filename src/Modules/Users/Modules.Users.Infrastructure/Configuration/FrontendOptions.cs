using System.Diagnostics.CodeAnalysis;

namespace Modules.Users.Infrastructure.Configuration;

[SuppressMessage(
    "Design",
    "CA1056:URI-like properties should not be strings",
    Justification = "Integration event contracts use language-neutral primitives")]
public class FrontendOptions
{
    public string BaseUrl { get; set; } = default!;
}
