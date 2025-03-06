using Database.DTO.ActiveQuestionnaire;

namespace API.DTO.Responses.ActiveQuestionnaire;

public record class ActiveQuestionnaireKeysetPaginationResult
{
    public List<ActiveQuestionnaireBase> ActiveQuestionnaireBases { get; set; } = [];
    public string? QueryCursor { get; set; }
    public int TotalCount { get; set; }
}
