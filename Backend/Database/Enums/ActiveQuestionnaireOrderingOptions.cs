using System.Text.Json.Serialization;
using Database.Attributes;
using Database.Extensions;

namespace Database.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ActiveQuestionnaireOrderingOptions
{
    [QueryMethod(nameof(IQueryableExtensions.OrderByTitleAsc))]
    TitleAsc,
    [QueryMethod(nameof(IQueryableExtensions.OrderByTitleDesc))]
    TitleDesc,
    [QueryMethod(nameof(IQueryableExtensions.OrderByActivatedAtAsc))]
    ActivatedAtAsc,
    [QueryMethod(nameof(IQueryableExtensions.OrderByActivatedAtDesc))]
    ActivatedAtDesc
}
