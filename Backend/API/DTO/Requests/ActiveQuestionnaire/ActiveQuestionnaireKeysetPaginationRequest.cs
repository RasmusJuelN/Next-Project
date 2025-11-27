
namespace API.DTO.Requests.ActiveQuestionnaire;

/// <summary>
/// Represents the base request model for keyset pagination of active questionnaires.
/// </summary>
/// <remarks>
/// This request is used to retrieve a paginated list of active questionnaires, supporting keyset pagination,
/// ordering, filtering by title, and cursor-based navigation.
/// </remarks>
public record class ActiveQuestionnaireKeysetPaginationRequestBase
{

    [DefaultValue(5)]
    public required int PageSize { get; set; }
    
    [DefaultValue(ActiveQuestionnaireOrderingOptions.ActivatedAtDesc)]
    public ActiveQuestionnaireOrderingOptions Order { get; set; } = ActiveQuestionnaireOrderingOptions.ActivatedAtDesc;
    
    public string? Title { get; set; }

    public Guid? ActiveQuestionnaireId { get; set; }

    public string? QueryCursor { get; set; }
}

/// <summary>
/// Represents a request for keyset pagination of active questionnaires for a student.
/// </summary>
/// <remarks>
/// This request allows students to retrieve their active questionnaires with options to filter by teacher,
/// whether the student has completed the questionnaire, and pagination settings.
/// </remarks>
public record class ActiveQuestionnaireKeysetPaginationRequestStudent : ActiveQuestionnaireKeysetPaginationRequestBase
{
    public string? Teacher { get; set; }
    public bool FilterStudentCompleted { get; set; }

}

/// <summary>
/// Represents a request for keyset pagination of active questionnaires for a teacher.
/// </summary>
/// <remarks>
/// This request allows teachers to retrieve active questionnaires assigned to their students,
/// wether the teacher or student has completed the questionnaire, and pagination settings.
/// </remarks>
public record class ActiveQuestionnaireKeysetPaginationRequestTeacher : ActiveQuestionnaireKeysetPaginationRequestBase
{
    public string? Student { get; set; }
    public bool FilterStudentCompleted { get; set; }
    public bool FilterTeacherCompleted { get; set; }
}

/// <summary>
/// Represents a request for keyset pagination of active questionnaires with full filtering options.
/// This request is intended for scenarios where both students and teachers can be filtered,
/// and allows for comprehensive pagination and ordering.
/// </summary>
/// <remarks>
/// This request extends the base pagination request to include additional filtering options
/// for both students and teachers, allowing for a more detailed and flexible query.
/// It is useful in administrative contexts or when detailed filtering is required.
/// </remarks>
public record class ActiveQuestionnaireKeysetPaginationRequestFull : ActiveQuestionnaireKeysetPaginationRequestBase
{
    public string? Teacher { get; set; }
    public string? Student { get; set; }
    public bool FilterStudentCompleted { get; set; }
    public bool FilterTeacherCompleted { get; set; }
}
