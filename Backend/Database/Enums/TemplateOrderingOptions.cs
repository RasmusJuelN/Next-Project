
namespace Database.Enums;

/// <summary>
/// Defines the available ordering options for template queries.
/// </summary>
/// <remarks>
/// This enum is decorated with <see cref="JsonStringEnumConverter"/> to enable JSON serialization
/// using string values instead of numeric values. Each enum value is associated with a specific
/// query method through the <see cref="QueryMethodAttribute"/> that defines how templates
/// should be ordered in database queries.
/// </remarks>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TemplateOrderingOptions
{
    [QueryMethod(nameof(IQueryableExtensions.OrderByTitleAsc))]
    TitleAsc,
    [QueryMethod(nameof(IQueryableExtensions.OrderByTitleDesc))]
    TitleDesc,
    [QueryMethod(nameof(IQueryableExtensions.OrderByCreatedAtAsc))]
    CreatedAtAsc,
    [QueryMethod(nameof(IQueryableExtensions.OrderByCreatedAtDesc))]
    CreatedAtDesc,
}
