
namespace API.DTO.Responses.QuestionnaireTemplate;

/// <summary>
/// Represents the result of a keyset pagination query for questionnaire templates.
/// </summary>
/// <remarks>
/// This record contains the paginated data along with metadata required for implementing
/// keyset (cursor-based) pagination, which provides consistent results even when the
/// underlying data changes between requests.
/// </remarks>
public record class TemplateKeysetPaginationResult
{
    /// <summary>
    /// Gets or sets the collection of questionnaire template base objects for the current page.
    /// </summary>
    /// <value>
    /// A list of <see cref="QuestionnaireTemplateBase"/> objects representing the templates
    /// in the current page. Defaults to an empty list if no templates are found.
    /// </value>
    public List<QuestionnaireTemplateBase> TemplateBases { get; set; } = [];

    /// <summary>
    /// Gets or sets the cursor for the next page of results.
    /// </summary>
    /// <value>
    /// A string representing the cursor position for retrieving the next set of results.
    /// This value should be passed as a parameter in subsequent pagination requests.
    /// Returns <c>null</c> when there are no more pages available.
    /// </value>
    public string? QueryCursor { get; set; }

    /// <summary>
    /// Gets or sets the total number of templates available across all pages.
    /// </summary>
    /// <value>
    /// An integer representing the total count of questionnaire templates that match
    /// the query criteria, regardless of pagination limits.
    /// </value>
    public int TotalCount { get; set; }
}
