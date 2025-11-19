using Database.DTO.User;
using Database.Enums;

namespace Database.DTO.ActiveQuestionnaire;

/// <summary>
/// Represents the base data transfer object for an active questionnaire containing core information
/// about a questionnaire that has been activated for completion by both student and teacher.
/// </summary>
/// <param name="Id">The unique identifier for the active questionnaire.</param>
/// <param name="Title">The title of the questionnaire.</param>
/// <param name="Description">An optional description providing additional details about the questionnaire.</param>
/// <param name="ActivatedAt">The date and time when the questionnaire was activated.</param>
/// <param name="Student">The student user associated with the questionnaire.</param>
/// <param name="Teacher">The teacher user associated with the questionnaire.</param>
/// <param name="StudentCompletedAt">The date and time when the student completed the questionnaire, if applicable.</param>
/// <param name="TeacherCompletedAt">The date and time when the teacher completed the questionnaire, if applicable.</param>
/// <param name="QuestionnaireType">The type of the active questionnaire.</param>
/// <remarks>
/// This record serves as the foundation for active questionnaire operations, tracking the lifecycle
/// of a questionnaire from activation through completion by both participants.
/// </remarks>
public record class ActiveQuestionnaireBase
{
    public required Guid Id { get; set; }
    public required Guid GroupId { get; set; }
    public Guid TemplateId { get; set; }
    public string? GroupName { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required DateTime ActivatedAt { get; set; }
    public required UserBase Student { get; set; }
    public required UserBase Teacher { get; set; }
    public required DateTime? StudentCompletedAt { get; set; }
    public required DateTime? TeacherCompletedAt { get; set; }
    public required ActiveQuestionnaireType QuestionnaireType { get; set; }
}
