using API.DTO.Responses.QuestionnaireTemplate;

namespace API.DTO.Responses.ActiveQuestionnaire;

public record class FetchActiveQuestionnaire : FetchActiveQuestionnaireBase
{
    public required List<FetchQuestion> Questions { get; set; }
}