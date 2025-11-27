
namespace Database.DTO.ActiveQuestionnaire;

/// <summary>
/// Represents the base information for an active questionnaire that is specific to a user.
/// This record contains the essential properties needed to identify and display
/// an active questionnaire in a user's context.
/// </summary>
/// <param name="Id">The unique identifier for the active questionnaire instance.</param>
/// <param name="Title">The title of the questionnaire that will be displayed to the user.</param>
/// <param name="Description">An optional description providing additional details about the questionnaire.</param>
/// <param name="ActivatedAt">The date and time when the questionnaire was activated for the user.</param>
public record class UserSpecificActiveQuestionnaireBase
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required DateTime ActivatedAt { get; set; }
}

/// <summary>
/// Represents an active questionnaire specific to a student user, extending the base user-specific questionnaire functionality.
/// </summary>
/// <remarks>
/// This record class tracks questionnaire completion status for student users, including when they completed the questionnaire.
/// It inherits all base properties from <see cref="UserSpecificActiveQuestionnaireBase"/> and adds student-specific completion tracking.
/// </remarks>
public record class StudentSpecificActiveQuestionnaire : UserSpecificActiveQuestionnaireBase
{
    public required DateTime? StudentCompletedAt { get; set; }
}

/// <summary>
/// Represents an active questionnaire specific to a teacher user, extending the base user-specific questionnaire functionality.
/// </summary>
/// <remarks>
/// This record class tracks questionnaire completion status for teacher users, including when they completed the questionnaire.
/// It inherits all properties from <see cref="StudentSpecificActiveQuestionnaire"/> and adds teacher-specific completion tracking.
/// </remarks>
public record class TeacherSpecificActiveQuestionnaire : StudentSpecificActiveQuestionnaire
{
    public required DateTime? TeacherCompletedAt { get; set; }
}

/// <summary>
/// Represents a complete student-specific active questionnaire that includes all questionnaire template questions.
/// This record extends the base StudentSpecificActiveQuestionnaire with the full set of questions.
/// </summary>
/// <remarks>
/// This record is typically used when a complete view of the questionnaire is needed,
/// including all associated questions from the questionnaire template.
/// </remarks>
public record class FullStudentSpecificActiveQuestionnaire : StudentSpecificActiveQuestionnaire
{
    public required List<QuestionnaireTemplateQuestion> Questions { get; set; }
}

/// <summary>
/// Represents a complete teacher-specific active questionnaire that includes all questionnaire template questions.
/// This record extends the base TeacherSpecificActiveQuestionnaire with the full set of questions.
/// </summary>
/// <remarks>
/// This record is typically used when a complete view of the questionnaire is needed,
/// including all associated questions from the questionnaire template.
/// </remarks>
public record class FullTeacherSpecificActiveQuestionnaire : TeacherSpecificActiveQuestionnaire
{
    public required List<QuestionnaireTemplateQuestion> Questions { get; set; }
}
