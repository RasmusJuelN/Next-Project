using Database.Enums;

namespace API.DTO.Requests.ActiveQuestionnaire
{
    public record class QuestionnaireGroupKeysetPaginationRequest
    {
        public required int PageSize { get; set; } = 5;
        public QuestionnaireGroupOrderingOptions Order { get; set; } = QuestionnaireGroupOrderingOptions.CreatedAtDesc;
        public string? Title { get; set; }
        public Guid? GroupId { get; set; }
        public string? QueryCursor { get; set; }
    }
}
