using System.Text.Json.Serialization;
using Database.Attributes;
using Database.Extensions;

namespace Database.Enums;

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
