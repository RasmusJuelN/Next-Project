using Database.DTO.QuestionnaireTemplate;
using Database.DTO.User;

namespace API.DTO.Responses.ActiveQuestionnaire;


public record class ActiveQuestionnaireUserSpecificBase
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required DateTime ActivatedAt { get; set; }
}

public record class ActiveQuestionnaireStudentBase : ActiveQuestionnaireUserSpecificBase
{
    public required UserBase Student { get; set; }
    public required DateTime? StudentCompletedAt { get; set; }
}

public record class ActiveQuestionnaireTeacherBase : ActiveQuestionnaireUserSpecificBase
{
    public required UserBase Student { get; set; }
    public required UserBase Teacher { get; set; }
    public required DateTime? StudentCompletedAt { get; set; }
    public required DateTime? TeacherCompletedAt { get; set; }
}

public record class ActiveQuestionnaireAdminBase : ActiveQuestionnaireUserSpecificBase
{
    public required UserBase Student { get; set; }
    public required UserBase Teacher { get; set; }
    public required DateTime? StudentCompletedAt { get; set; }
    public required DateTime? TeacherCompletedAt { get; set; }
}

//###################################################//

public record class ActiveQuestionnaireStudentFull : ActiveQuestionnaireStudentBase
{
    public required List<QuestionnaireTemplateQuestion> Questions { get; set; }
}

public record class ActiveQuestionnaireTeacherFull : ActiveQuestionnaireTeacherBase
{
    public required List<QuestionnaireTemplateQuestion> Questions { get; set; }
}

public record class ActiveQuestionnaireAdminFull : ActiveQuestionnaireAdminBase
{
    public required List<QuestionnaireTemplateQuestion> Questions { get; set; }
}