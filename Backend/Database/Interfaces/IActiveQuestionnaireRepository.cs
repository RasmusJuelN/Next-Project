using Database.DTO.ActiveQuestionnaire;
using Database.Enums;

namespace Database.Interfaces;

/// <summary>
/// Defines the contract for active questionnaire repository operations.
/// Manages the lifecycle of questionnaires from activation through completion, including answer submission and response retrieval.
/// </summary>
public interface IActiveQuestionnaireRepository
{
    /// <summary>
    /// Retrieves basic information about an active questionnaire.
    /// </summary>
    /// <param name="id">The unique identifier of the active questionnaire.</param>
    /// <returns>An ActiveQuestionnaireBase DTO containing essential questionnaire information.</returns>
    /// <exception cref="ArgumentException">Thrown when the questionnaire ID is not found.</exception>
    Task<ActiveQuestionnaireBase> GetActiveQuestionnaireBaseAsync(Guid id);

    /// <summary>
    /// Retrieves complete information about an active questionnaire including all questions and submitted answers.
    /// </summary>
    /// <param name="id">The unique identifier of the active questionnaire.</param>
    /// <returns>A complete ActiveQuestionnaire DTO with questions and answers.</returns>
    /// <exception cref="ArgumentException">Thrown when the questionnaire ID is not found.</exception>
    Task<ActiveQuestionnaire> GetFullActiveQuestionnaireAsync(Guid id);

    /// <summary>
    /// Creates a new active questionnaire instance from a template and assigns it to specific student and teacher.
    /// </summary>
    /// <param name="questionnaireTemplateId">The ID of the questionnaire template to activate.</param>
    /// <param name="studentId">The ID of the student who will complete the questionnaire.</param>
    /// <param name="teacherId">The ID of the teacher who will review the questionnaire.</param>
    /// <param name="groupId">The ID of the group associated with this questionnaire activation.</param>
    /// <param name="activeQuestionnaireType">The type of active questionnaire being created.</param>
    /// <returns>The newly created ActiveQuestionnaire instance.</returns>
    /// <exception cref="ArgumentException">Thrown when any of the provided IDs are not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the template is locked or cannot be activated.</exception>
    Task<ActiveQuestionnaire> ActivateQuestionnaireAsync(
        Guid questionnaireTemplateId,
        Guid studentId,
        Guid teacherId,
        Guid groupId,
        ActiveQuestionnaireType activeQuestionnaireType);

    /// <summary>
    /// Performs paginated retrieval of active questionnaires with advanced filtering and sorting options using keyset pagination.
    /// </summary>
    /// <param name="amount">The number of questionnaires to retrieve per page.</param>
    /// <param name="sortOrder">The ordering criteria for the results.</param>
    /// <param name="cursorIdPosition">Optional cursor ID for pagination continuation.</param>
    /// <param name="cursorActivatedAtPosition">Optional cursor activation timestamp for pagination continuation.</param>
    /// <param name="titleQuery">Optional filter by questionnaire title (partial match).</param>
    /// <param name="student">Optional filter by student name or identifier.</param>
    /// <param name="teacher">Optional filter by teacher name or identifier.</param>
    /// <param name="idQuery">Optional filter by specific questionnaire ID.</param>
    /// <param name="userId">Optional filter to show questionnaires assigned to a specific user.</param>
    /// <param name="onlyStudentCompleted">When true, shows only questionnaires completed by students.</param>
    /// <param name="onlyTeacherCompleted">When true, shows only questionnaires completed by teachers.</param>
    /// <param name="pendingStudent">When true, shows only questionnaires pending student completion.</param>
    /// <param name="pendingTeacher">When true, shows only questionnaires pending teacher completion.</param>
    /// <param name="questionnaireType">Optional filter by the type of active questionnaire.</param>
    /// <returns>A tuple containing the list of questionnaires and the total count matching the criteria.</returns>
    /// <remarks>
    /// Uses keyset pagination for consistent performance with large datasets. Combine cursor parameters for proper pagination.
    /// Filtering options can be combined to create complex queries for specific questionnaire subsets.
    /// </remarks>
    Task<(List<ActiveQuestionnaireBase>, int)> PaginationQueryWithKeyset(
        int amount,
        ActiveQuestionnaireOrderingOptions sortOrder,
        Guid? cursorIdPosition = null,
        DateTime? cursorActivatedAtPosition = null,
        string? titleQuery = null,
        string? student = null,
        string? teacher = null,
        Guid? idQuery = null,
        Guid? userId = null,
        bool onlyStudentCompleted = false,
        bool onlyTeacherCompleted = false,
        bool pendingStudent = false,         // NEW
        bool pendingTeacher = false,
        ActiveQuestionnaireType? questionnaireType = null);


    /// <summary>
    /// Submits answers for a specific active questionnaire on behalf of a user.
    /// </summary>
    /// <param name="activeQuestionnaireId">The ID of the active questionnaire to submit answers for.</param>
    /// <param name="userId">The ID of the user submitting the answers.</param>
    /// <param name="submission">The answer submission containing responses to questionnaire questions.</param>
    /// <exception cref="ArgumentException">Thrown when questionnaire or user ID is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the questionnaire is already completed or locked.</exception>
    /// <remarks>
    /// This method handles the submission of answers and updates completion status accordingly.
    /// </remarks>
    Task AddAnswers(Guid activeQuestionnaireId, Guid userId, AnswerSubmission submission);

    /// <summary>
    /// Checks if a specific user has submitted answers for an active questionnaire.
    /// </summary>
    /// <param name="userId">The ID of the user to check.</param>
    /// <param name="activeQuestionnaireId">The ID of the active questionnaire.</param>
    /// <returns>True if the user has submitted answers, false otherwise.</returns>
    /// <remarks>
    /// Used to prevent duplicate submissions and to check completion status for workflow management.
    /// </remarks>
    Task<bool> HasUserSubmittedAnswer(Guid userId, Guid activeQuestionnaireId);

    /// <summary>
    /// Checks if an active questionnaire is completely finished by both student and teacher.
    /// </summary>
    /// <param name="activeQuestionnaireId">The ID of the active questionnaire to check.</param>
    /// <returns>True if both parties have completed their portions, false otherwise.</returns>
    /// <remarks>
    /// A questionnaire is considered complete when both student and teacher have submitted their responses.
    /// </remarks>
    Task<bool> IsActiveQuestionnaireComplete(Guid activeQuestionnaireId);

    /// <summary>
    /// Checks if a specific user has completed their portion of an active questionnaire.
    /// </summary>
    /// <param name="activeQuestionnaireId">The ID of the active questionnaire to check.</param>
    /// <param name="userId">The ID of the user to check completion status for.</param>
    /// <returns>True if the specified user has completed their portion, false otherwise.</returns>
    /// <remarks>
    /// Used to track individual completion status and manage user-specific workflow states.
    /// </remarks>
    Task<bool> IsActiveQuestionnaireComplete(Guid activeQuestionnaireId, Guid userId);

    /// <summary>
    /// Retrieves the complete response data for an active questionnaire including all submitted answers.
    /// </summary>
    /// <param name="id">The ID of the active questionnaire.</param>
    /// <returns>A FullResponse DTO containing all questionnaire data and submitted answers.</returns>
    /// <exception cref="ArgumentException">Thrown when the questionnaire ID is not found.</exception>
    /// <remarks>
    /// Used for generating reports, reviews, and comprehensive questionnaire analysis.
    /// Contains sensitive data and should only be accessible to authorized users.
    /// </remarks>
    Task<FullResponse> GetFullResponseAsync(Guid id);

    /// <summary>
    /// Retrieves all pending active questionnaires for a specific user.
    /// </summary>
    /// <param name="id">The ID of the user to get pending questionnaires for.</param>
    /// <returns>A list of ActiveQuestionnaireBase DTOs representing pending questionnaires.</returns>
    /// <remarks>
    /// Pending questionnaires are those where the user has not yet completed their portion.
    /// Used to display user dashboards and task lists.
    /// </remarks>
    Task<List<ActiveQuestionnaireBase>> GetPendingActiveQuestionnaires(Guid id);

    Task<List<FullStudentRespondsDate>> GetResponsesFromStudentAndTemplateAsync(Guid studentid, Guid templateid);
    Task<List<FullStudentRespondsDate>> GetResponsesFromStudentAndTemplateWithDateAsync(Guid studentid, Guid templateid);

    /// <summary>
    /// Retrieves the response history for a specific student and questionnaire template.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student whose response history to retrieve.</param>
    /// <param name="teacherId">The unique identifier of the teacher making the request.</param>
    /// <param name="templateId">The unique identifier of the questionnaire template.</param>
    /// <returns>
    /// A <see cref="StudentResultHistory"/> object containing the student's response history for the specified template,
    /// or null if no history is found.
    /// </returns>
    /// <remarks>
    /// This method retrieves all historical responses from a student for a specific questionnaire template,
    /// providing teachers with insight into student progress over time.
    /// </remarks>
    Task<StudentResultHistory?> GetResponseHistoryAsync(Guid studentId, Guid teacherId, Guid templateId);

    Task<SurveyResponseSummary> GetAnonymisedResponses(Guid templateId, List<Guid> users, List<Guid> groups);

    Task<List<ActiveQuestionnaireBase>> GetCompletedQuestionnairesByGroupAsync(Guid activeQuestionnaireId);

    /// <summary>
    /// Determines whether the specified active questionnaire is of anonymous type.
    /// </summary>
    /// <param name="activeQuestionnaireId">The unique identifier of the active questionnaire to check.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean value:
    /// <c>true</c> if the active questionnaire is anonymous; otherwise, <c>false</c>.
    /// Returns <c>false</c> if no questionnaire with the specified ID is found.
    /// </returns>
    Task<bool> IsActiveQuestionnaireAnonymous(Guid activeQuestionnaireId);
}
