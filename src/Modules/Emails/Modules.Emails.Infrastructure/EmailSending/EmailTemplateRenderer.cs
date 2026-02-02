using System.Text.RegularExpressions;

namespace Modules.Emails.Infrastructure.EmailSending;

/// <summary>
/// Replaces {{Key}} placeholders in a template string with values from the variables dictionary.
/// Unknown placeholders are left untouched so missing data doesn't silently produce empty spots.
/// </summary>
internal static class EmailTemplateRenderer
{
    private static readonly Regex PlaceholderRegex = new(@"\{\{(\w+)\}\}", RegexOptions.Compiled);

    private static Dictionary<string, string> _defaultVariables = new();

    public static void SetDefaults(Dictionary<string, string>? defaults)
    {
        _defaultVariables = defaults ?? new Dictionary<string, string>();
    }

    /// <param name="template">Raw HTML with {{Key}} placeholders.</param>
    /// <param name="variables">Key-value pairs to substitute.</param>
    /// <returns>The rendered HTML string.</returns>
    public static string Render(string template, Dictionary<string, string>? variables)
    {
        if (string.IsNullOrEmpty(template))
            return template;

        var merged = new Dictionary<string, string>(_defaultVariables, StringComparer.OrdinalIgnoreCase);

        if (variables != null)
        {
            foreach (var kv in variables)
            {
                merged[kv.Key] = kv.Value;
            }
        }

        return PlaceholderRegex.Replace(template, match =>
        {
            var key = match.Groups[1].Value;
            return merged.TryGetValue(key, out var value) ? value : match.Value;
        });
    }
}
