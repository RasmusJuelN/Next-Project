using System.ComponentModel;
using Database.Enums;

namespace API.DTO.Requests.ActiveQuestionnaire;

public record class ActiveQuestionnaireKeysetPaginationRequestBase
{
    /// <summary>
    /// How many items the pagination query should return.
    /// </summary>
    [DefaultValue(5)]
    public required int PageSize { get; set; }
    
    /// <summary>
    /// The order in which the items should be paginated and queried in.
    /// </summary>
    [DefaultValue(ActiveQuestionnaireOrderingOptions.ActivatedAtDesc)]
    public ActiveQuestionnaireOrderingOptions Order { get; set; } = ActiveQuestionnaireOrderingOptions.ActivatedAtDesc;
    
    /// <summary>
    /// The title to search for in the templates.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The partial or exact ID of a template.
    /// </summary>
    public Guid? ActiveQuestionnaireId { get; set; }

    /// <summary>
    /// The cursor for where the query should start/resume from
    /// </summary>
    public string? QueryCursor { get; set; }
}

public record class ActiveQuestionnaireKeysetPaginationRequestStudent : ActiveQuestionnaireKeysetPaginationRequestBase
{
    public string? Teacher { get; set; }    
}

public record class ActiveQuestionnaireKeysetPaginationRequestTeacher : ActiveQuestionnaireKeysetPaginationRequestBase
{
    public string? Student { get; set; }
}

public record class ActiveQuestionnaireKeysetPaginationRequestFull : ActiveQuestionnaireKeysetPaginationRequestBase
{
    public string? Teacher { get; set; }
    public string? Student { get; set; }
}
