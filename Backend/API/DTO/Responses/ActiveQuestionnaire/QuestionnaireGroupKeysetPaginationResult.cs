using Database.DTO.ActiveQuestionnaire;

namespace API.DTO.Responses.ActiveQuestionnaire
{
    public record class QuestionnaireGroupKeysetPaginationResult
    {
        public required List<QuestionnaireGroupResult> Groups { get; set; }
        public string? QueryCursor { get; set; }
        public int TotalCount { get; set; }
    }
}
