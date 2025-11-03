using Database.DTO.QuestionnaireTemplate;
using Database.DTO.User;

namespace API.DTO.Responses.ActiveQuestionnaire;


/// <summary>
/// Represents the base information for an active questionnaire specific to a user.
/// Contains essential questionnaire details including identification, metadata, and activation timestamp.
/// </summary>
/// <remarks>
/// This record serves as a base class for user-specific active questionnaire data transfer objects.
/// It includes the core properties needed to identify and display basic questionnaire information
/// to users in the context of active questionnaires.
/// </remarks>
public record class ActiveQuestionnaireUserSpecificBase
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required DateTime ActivatedAt { get; set; }
}

/// <summary>
/// Represents an active questionnaire with student-specific information including completion status.
/// Extends the base user-specific active questionnaire with student details and completion timestamp.
/// </summary>
public record class ActiveQuestionnaireStudentBase : ActiveQuestionnaireUserSpecificBase
{
    public required UserBase Student { get; set; }
    public required DateTime? StudentCompletedAt { get; set; }
}

/// <summary>
/// Represents the teacher-specific view of an active questionnaire, containing information about both student and teacher participation.
/// </summary>
/// <remarks>
/// This record extends the base user-specific active questionnaire data with additional details about
/// the student-teacher relationship and completion status for both parties involved in the questionnaire process.
/// </remarks>
public record class ActiveQuestionnaireTeacherBase : ActiveQuestionnaireUserSpecificBase
{
    public required UserBase Student { get; set; }
    public required UserBase Teacher { get; set; }
    public required DateTime? StudentCompletedAt { get; set; }
    public required DateTime? TeacherCompletedAt { get; set; }
    public string? GroupName { get; set; }
}

/// <summary>
/// Represents an active questionnaire with administrative details including student and teacher information.
/// Extends the user-specific base to include completion tracking for both student and teacher participants.
/// </summary>
/// <remarks>
/// This DTO is used for administrative views where both student and teacher completion status
/// and participant details need to be displayed together.
/// </remarks>
public record class ActiveQuestionnaireAdminBase : ActiveQuestionnaireUserSpecificBase
{
    public required UserBase Student { get; set; }
    public required UserBase Teacher { get; set; }
    public required DateTime? StudentCompletedAt { get; set; }
    public required DateTime? TeacherCompletedAt { get; set; }
}

//###################################################//

/// <summary>
/// Represents a complete active questionnaire for a student, including all associated questions.
/// Extends the base student questionnaire with the full question details.
/// </summary>
/// <remarks>
/// This record is typically used when a student needs to view or complete a questionnaire,
/// providing access to all the questions that need to be answered.
/// </remarks>
public record class ActiveQuestionnaireStudentFull : ActiveQuestionnaireStudentBase
{
    public required List<QuestionnaireTemplateQuestion> Questions { get; set; }
}

/// <summary>
/// Represents a complete active questionnaire for a teacher, including all associated questions.
/// Extends the base teacher questionnaire with the full question details.
/// </summary>
/// <remarks>
/// This record is typically used when a teacher needs to view or complete a questionnaire,
/// providing access to all the questions that need to be answered.
/// </remarks>
public record class ActiveQuestionnaireTeacherFull : ActiveQuestionnaireTeacherBase
{
    public required List<QuestionnaireTemplateQuestion> Questions { get; set; }
}

/// <summary>
/// Represents a complete active questionnaire data transfer object for administrative purposes,
/// containing all questionnaire details including the full list of questions.
/// </summary>
/// <remarks>
/// This record extends <see cref="ActiveQuestionnaireAdminBase"/> by adding the complete
/// collection of questions associated with the questionnaire template.
/// </remarks>
public record class ActiveQuestionnaireAdminFull : ActiveQuestionnaireAdminBase
{
    public required List<QuestionnaireTemplateQuestion> Questions { get; set; }
}