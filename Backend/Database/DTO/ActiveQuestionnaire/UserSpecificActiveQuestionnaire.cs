using Database.DTO.QuestionnaireTemplate;

namespace Database.DTO.ActiveQuestionnaire;

public record class UserSpecificActiveQuestionnaireBase
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required DateTime ActivatedAt { get; set; }
    public required DateTime? CompletedAt { get; set; }   
}

public record class UserSpecificActiveQuestionnaire : UserSpecificActiveQuestionnaireBase
{
    public required List<QuestionnaireTemplateQuestion> Questions { get; set; }
}