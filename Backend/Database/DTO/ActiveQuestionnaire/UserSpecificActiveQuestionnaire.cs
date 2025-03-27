using Database.DTO.QuestionnaireTemplate;

namespace Database.DTO.ActiveQuestionnaire;

public record class UserSpecificActiveQuestionnaireBase
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required DateTime ActivatedAt { get; set; }
}

public record class StudentSpecificActiveQuestionnaire : UserSpecificActiveQuestionnaireBase
{
    public required DateTime? StudentCompletedAt { get; set; }
}

public record class TeacherSpecificActiveQuestionnaire : StudentSpecificActiveQuestionnaire
{
    public required DateTime? TeacherCompletedAt { get; set; }    
}

public record class FullStudentSpecificActiveQuestionnaire : StudentSpecificActiveQuestionnaire
{
    public required List<QuestionnaireTemplateQuestion> Questions { get; set; }
}

public record class FullTeacherSpecificActiveQuestionnaire : TeacherSpecificActiveQuestionnaire
{
    public required List<QuestionnaireTemplateQuestion> Questions { get; set; }
}
