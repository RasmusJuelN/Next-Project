
namespace API.Utils;

/// <summary>
/// A parameter transformer that converts PascalCase or camelCase parameter names to kebab-case (slug format).
/// This transformer is used to convert action and controller names in URLs to a more web-friendly format.
/// </summary>
/// <remarks>
/// This class implements <see cref="IOutboundParameterTransformer"/> to transform outbound URL parameters.
/// For example, "MyController" becomes "my-controller" and "GetUserById" becomes "get-user-by-id".
/// </remarks>
/// <example>
/// Usage in Startup.cs or Program.cs:
/// <code>
/// services.Configure&lt;RouteOptions&gt;(options =>
/// {
///     options.ConstraintMap.Add("slugify", typeof(SlugifyParameterTransformer));
/// });
/// </code>
/// </example>
public partial class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    /// <summary>
    /// Transforms an outbound route parameter value by converting it to a slug format.
    /// Converts PascalCase/camelCase strings to lowercase kebab-case by inserting hyphens
    /// between words and converting to lowercase.
    /// </summary>
    /// <param name="value">The route parameter value to transform. Can be null.</param>
    /// <returns>
    /// A lowercase, hyphen-separated string representation of the input value,
    /// or null if the input value is null.
    /// </returns>
    /// <example>
    /// "MyController" becomes "my-controller"
    /// "UserProfile" becomes "user-profile"
    /// </example>
    public string? TransformOutbound(object? value)
    {
        if (value == null) return null;

        return MyRegex().Replace(value.ToString()!, "$1-$2").ToLower();
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex MyRegex();
}
