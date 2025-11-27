
namespace Database.Enums;

/// <summary>
/// Defines the available ordering options for active questionnaires.
/// </summary>
/// <remarks>
/// This enum is serialized as string values in JSON and each option maps to a specific query method
/// for ordering questionnaire collections. The ordering can be applied by title or activation date
/// in both ascending and descending order.
/// </remarks>
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
