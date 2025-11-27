
namespace API.Extensions;

/// <summary>
/// Provides extension methods for converting ActiveQuestionnaireBase objects to different DTO representations
/// based on user roles (Student, Teacher, Admin).
/// </summary>
/// <remarks>
/// This class contains methods that transform ActiveQuestionnaireBase entities into role-specific DTOs,
/// exposing different levels of information based on the target user type.
/// </remarks>
public static class ActiveQuestionnaireExtensions
{
    /// <summary>
    /// Converts an ActiveQuestionnaireBase object to an ActiveQuestionnaireStudentBase DTO.
    /// </summary>
    /// <param name="activeQuestionnaire">The active questionnaire to convert.</param>
    /// <returns>
    /// An ActiveQuestionnaireStudentBase DTO containing student-relevant information including
    /// questionnaire details, activation time, student information, and student completion status.
    /// </returns>
    /// <remarks>
    /// This method creates a student-focused view of the questionnaire data, excluding teacher-specific
    /// information such as teacher details and teacher completion status.
    /// </remarks>
    public static ActiveQuestionnaireStudentBase ToActiveQuestionnaireStudentDTO(this ActiveQuestionnaireBase activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            Student = activeQuestionnaire.Student,
            StudentCompletedAt = activeQuestionnaire.StudentCompletedAt
        };
    }

    /// <summary>
    /// Converts an ActiveQuestionnaireBase object to an ActiveQuestionnaireTeacherBase DTO.
    /// </summary>
    /// <param name="activeQuestionnaire">The active questionnaire to convert.</param>
    /// <returns>
    /// An ActiveQuestionnaireTeacherBase DTO containing comprehensive information including
    /// questionnaire details, activation time, both student and teacher information, and completion statuses.
    /// </returns>
    /// <remarks>
    /// This method creates a teacher-focused view of the questionnaire data, providing access to both
    /// student and teacher information for monitoring and assessment purposes.
    /// </remarks>
    public static ActiveQuestionnaireTeacherBase ToActiveQuestionnaireTeacherDTO(this ActiveQuestionnaireBase activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            GroupName = activeQuestionnaire.GroupName,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            Student = activeQuestionnaire.Student,
            StudentCompletedAt = activeQuestionnaire.StudentCompletedAt,
            Teacher = activeQuestionnaire.Teacher,
            TeacherCompletedAt = activeQuestionnaire.TeacherCompletedAt
        };
    }

    /// <summary>
    /// Converts an ActiveQuestionnaireBase object to an ActiveQuestionnaireAdminBase DTO.
    /// </summary>
    /// <param name="activeQuestionnaire">The active questionnaire to convert.</param>
    /// <returns>
    /// An ActiveQuestionnaireAdminBase DTO containing full administrative information including
    /// questionnaire details, activation time, both student and teacher information, and completion statuses.
    /// </returns>
    /// <remarks>
    /// This method creates an administrative view of the questionnaire data, providing complete access
    /// to all information for system administration and oversight purposes.
    /// </remarks>
    public static ActiveQuestionnaireAdminBase ToActiveQuestionnaireAdminDTO(this ActiveQuestionnaireBase activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            Student = activeQuestionnaire.Student,
            StudentCompletedAt = activeQuestionnaire.StudentCompletedAt,
            Teacher = activeQuestionnaire.Teacher,
            TeacherCompletedAt = activeQuestionnaire.TeacherCompletedAt
        };
    }
}
