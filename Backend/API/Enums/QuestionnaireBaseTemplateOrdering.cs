using System.Text.Json.Serialization;
using API.Attributes;
using API.Extensions;

namespace API.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum QuestionnaireBaseTemplateOrdering
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
