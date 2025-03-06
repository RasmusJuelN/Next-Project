using Database.DTO.QuestionnaireTemplate;

namespace Database.DTO.ActiveQuestionnaire;

public record class ActiveQuestionnaire : ActiveQuestionnaireBase
{
    public required List<QuestionnaireTemplateQuestion> Questions { get; set; }
}
