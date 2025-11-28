
namespace Database.Extensions;

/// <summary>
/// Provides extension methods for mapping active questionnaire models to user-specific DTOs.
/// This mapper creates role-based views of active questionnaires with completion status tailored to student or teacher perspectives.
/// </summary>
public static class UserSpecificActiveQuestionnaireMapper
{
    /// <summary>
    /// Converts an ActiveQuestionnaireModel to a StudentSpecificActiveQuestionnaire DTO from the student's perspective.
    /// </summary>
    /// <param name="activeQuestionnaire">The ActiveQuestionnaireModel from the database.</param>
    /// <returns>A StudentSpecificActiveQuestionnaire DTO showing the questionnaire state as seen by the student.</returns>
    /// <remarks>
    /// This method creates a student-focused view of the active questionnaire, showing the student's completion status.
    /// The completion timestamp reflects when the student finished their portion of the questionnaire.
    /// Used to display questionnaire status to students in their personal dashboard.
    /// </remarks>
    public static StudentSpecificActiveQuestionnaire ToBaseDTOAsStudent(this ActiveQuestionnaireModel activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            StudentCompletedAt = activeQuestionnaire.StudentCompletedAt
        };
    }

    /// <summary>
    /// Converts an ActiveQuestionnaireModel to a TeacherSpecificActiveQuestionnaire DTO from the teacher's perspective.
    /// </summary>
    /// <param name="activeQuestionnaire">The ActiveQuestionnaireModel from the database.</param>
    /// <returns>A TeacherSpecificActiveQuestionnaire DTO showing the questionnaire state with both student and teacher completion status.</returns>
    /// <remarks>
    /// This method creates a teacher-focused view of the active questionnaire, including both the student's completion status
    /// and the teacher's completion status. This comprehensive view allows teachers to see the full questionnaire lifecycle,
    /// tracking when students completed their responses and when teachers completed their review or assessment.
    /// Used to display questionnaire status to teachers in their assessment dashboard.
    /// </remarks>
    public static TeacherSpecificActiveQuestionnaire ToBaseDTOAsTeacher(this ActiveQuestionnaireModel activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            StudentCompletedAt = activeQuestionnaire.StudentCompletedAt,
            TeacherCompletedAt = activeQuestionnaire.TeacherCompletedAt
        };
    }
}
