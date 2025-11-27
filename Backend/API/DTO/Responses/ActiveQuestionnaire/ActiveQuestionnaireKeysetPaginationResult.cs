
namespace API.DTO.Responses.ActiveQuestionnaire;

/// <summary>
/// Represents the base pagination result for active questionnaire data using keyset pagination.
/// </summary>
/// <remarks>
/// This record provides essential pagination information including a cursor for navigation
/// and the total count of items available in the dataset.
/// </remarks>
public record class ActiveQuestionnaireKeysetPaginationResultBase
{
    public string? QueryCursor { get; set; }
    public int TotalCount { get; set; }
}

/// <summary>
/// Represents a keyset pagination result containing active questionnaires specifically for students.
/// Inherits from <see cref="ActiveQuestionnaireKeysetPaginationResultBase"/> and provides
/// a collection of student-specific active questionnaire data.
/// </summary>
public record class ActiveQuestionnaireKeysetPaginationResultStudent : ActiveQuestionnaireKeysetPaginationResultBase
{
    public required List<ActiveQuestionnaireStudentBase> ActiveQuestionnaireBases { get; set; }
}

/// <summary>
/// Represents a keyset pagination result containing active questionnaires specifically for teachers.
/// Inherits from <see cref="ActiveQuestionnaireKeysetPaginationResultBase"/> and includes 
/// a collection of teacher-specific active questionnaire data.
/// </summary>
public record class ActiveQuestionnaireKeysetPaginationResultTeacher : ActiveQuestionnaireKeysetPaginationResultBase
{
    public required List<ActiveQuestionnaireTeacherBase> ActiveQuestionnaireBases { get; set; }
}

/// <summary>
/// Represents a keyset pagination result containing active questionnaires specifically for admins.
/// Inherits from <see cref="ActiveQuestionnaireKeysetPaginationResultBase"/> and includes 
/// a collection of admin-specific active questionnaire data.
/// </summary>
public record class ActiveQuestionnaireKeysetPaginationResultAdmin : ActiveQuestionnaireKeysetPaginationResultBase
{
    public required List<ActiveQuestionnaireBase> ActiveQuestionnaireBases { get; set; }
}
