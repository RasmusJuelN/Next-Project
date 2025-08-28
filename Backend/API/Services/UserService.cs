using API.DTO.LDAP;
using API.DTO.Requests.ActiveQuestionnaire;
using API.DTO.Requests.User;
using API.DTO.Responses.ActiveQuestionnaire;
using API.DTO.Responses.User;
using API.Extensions;
using API.Interfaces;
using Database.DTO.ActiveQuestionnaire;

namespace API.Services;

/// <summary>
/// Provides user management services including LDAP integration and user-specific data retrieval.
/// This service acts as a bridge between the authentication system and the application's data layer,
/// handling user queries, role-based data access, and questionnaire assignment for different user types.
/// </summary>
/// <remarks>
/// The service integrates multiple data sources:
/// <list type="bullet">
/// <item><description>LDAP directory for user authentication and directory queries</description></item>
/// <item><description>Local database for questionnaire assignments and responses</description></item>
/// <item><description>Role-based filtering for students, teachers, and administrators</description></item>
/// </list>
/// It provides a unified interface for user operations while maintaining security boundaries.
/// </remarks>
public class UserService(IAuthenticationBridge authenticationBridge, IUnitOfWork unitOfWork)
{
    private readonly IAuthenticationBridge _authenticationBridge = authenticationBridge;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// Queries the LDAP directory for users with pagination support and role-based filtering.
    /// </summary>
    /// <param name="request">The pagination and filtering parameters for the user query.</param>
    /// <returns>
    /// A paginated result containing user information, session management data, and navigation metadata.
    /// </returns>
    /// <remarks>
    /// This method provides administrative functionality for browsing and searching users in the LDAP directory.
    /// It supports:
    /// <list type="bullet">
    /// <item><description>Role-based filtering (student, teacher, admin)</description></item>
    /// <item><description>Search term matching against user attributes</description></item>
    /// <item><description>Session-based pagination for large result sets</description></item>
    /// <item><description>Automatic conversion from LDAP format to application DTOs</description></item>
    /// </list>
    /// The returned session ID should be used for subsequent page requests to maintain query context.
    /// </remarks>
    /// <exception cref="API.Exceptions.LDAPException">Thrown when LDAP connection or query operations fail.</exception>
    /// <exception cref="ArgumentException">Thrown when request parameters are invalid.</exception>
    public UserQueryPaginationResult QueryLDAPUsersWithPagination(UserQueryPagination request)
    {
        (List<BasicUserInfoWithObjectGuid> ldapUsers, string sessionId, bool hasMore) = _authenticationBridge.SearchUserPagination<BasicUserInfoWithObjectGuid>(request.User, request.Role.ToString(), request.PageSize, request.SessionId);

        return new()
        {
            UserBases = [.. ldapUsers.Select(u => u.ToUserBaseDto())],
            SessionId = sessionId,
            HasMore = hasMore
        };
    }

    /// <summary>
    /// Retrieves active questionnaires for a student user with pagination support.
    /// </summary>
    /// <param name="request">The pagination request containing filtering and cursor information.</param>
    /// <param name="userId">The unique identifier of the student user.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// paginated active questionnaires specific to the student context.
    /// </returns>
    /// <remarks>
    /// This method returns questionnaires that are currently active and accessible
    /// to the specified student. The results include questionnaire metadata
    /// and submission status. Pagination is implemented
    /// using keyset pagination for efficient large dataset handling.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when request parameters are invalid.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user lacks student privileges.</exception>
    public async Task<ActiveQuestionnaireKeysetPaginationResultStudent> GetActiveQuestionnairesForStudent(ActiveQuestionnaireKeysetPaginationRequestStudent request, Guid userId)
    {
        DateTime? cursorActivatedAt = null;
        Guid? cursorId = null;

        if (!string.IsNullOrEmpty(request.QueryCursor))
        {
            cursorActivatedAt = DateTime.Parse(request.QueryCursor.Split('_')[0]);
            cursorId = Guid.Parse(request.QueryCursor.Split('_')[1]);
        }

        (List<ActiveQuestionnaireBase> activeQuestionnaireBases, int totalCount) = await _unitOfWork.ActiveQuestionnaire.PaginationQueryWithKeyset(
            request.PageSize,
            request.Order,
            cursorId,
            cursorActivatedAt,
            request.Title,
            student: request.Teacher,
            idQuery: request.ActiveQuestionnaireId,
            userId: userId,
            onlyStudentCompleted: request.FilterStudentCompleted);
        
        ActiveQuestionnaireBase? lastActiveQuestionnaire = activeQuestionnaireBases.Count != 0 ? activeQuestionnaireBases.Last() : null;

        string? queryCursor = null;
        if (lastActiveQuestionnaire is not null)
        {
            queryCursor = $"{lastActiveQuestionnaire.ActivatedAt:O}_{lastActiveQuestionnaire.Id}";
        }

        return new()
        {
            ActiveQuestionnaireBases = [.. activeQuestionnaireBases.Select(a => a.ToActiveQuestionnaireStudentDTO())],
            QueryCursor = queryCursor,
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Retrieves active questionnaires for a teacher user with pagination support.
    /// </summary>
    /// <param name="request">The pagination request containing filtering and cursor information.</param>
    /// <param name="userId">The unique identifier of the teacher user.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// paginated active questionnaires specific to the teacher context.
    /// </returns>
    /// <remarks>
    /// This method returns questionnaires that are currently active and accessible
    /// to the specified teacher. The results include questionnaire metadata
    /// and submission status. Pagination is implemented
    /// using keyset pagination for efficient large dataset handling.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when request parameters are invalid.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user lacks teacher privileges.</exception>
    public async Task<ActiveQuestionnaireKeysetPaginationResultTeacher> GetActiveQuestionnairesForTeacher(ActiveQuestionnaireKeysetPaginationRequestTeacher request, Guid userId)
    {
        DateTime? cursorActivatedAt = null;
        Guid? cursorId = null;

        if (!string.IsNullOrEmpty(request.QueryCursor))
        {
            cursorActivatedAt = DateTime.Parse(request.QueryCursor.Split('_')[0]);
            cursorId = Guid.Parse(request.QueryCursor.Split('_')[1]);
        }

        (List<ActiveQuestionnaireBase> activeQuestionnaireBases, int totalCount) = await _unitOfWork.ActiveQuestionnaire.PaginationQueryWithKeyset(
            request.PageSize,
            request.Order,
            cursorId,
            cursorActivatedAt,
            request.Title,
            student: request.Student,
            idQuery: request.ActiveQuestionnaireId,
            userId: userId,
            onlyStudentCompleted: request.FilterStudentCompleted,
            onlyTeacherCompleted: request.FilterTeacherCompleted);
        
        ActiveQuestionnaireBase? lastActiveQuestionnaire = activeQuestionnaireBases.Count != 0 ? activeQuestionnaireBases.Last() : null;

        string? queryCursor = null;
        if (lastActiveQuestionnaire is not null)
        {
            queryCursor = $"{lastActiveQuestionnaire.ActivatedAt:O}_{lastActiveQuestionnaire.Id}";
        }

        return new()
        {
            ActiveQuestionnaireBases = [.. activeQuestionnaireBases.Select(a => a.ToActiveQuestionnaireTeacherDTO())],
            QueryCursor = queryCursor,
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Retrieves a list of active questionnaires that are pending completion by the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to check for pending questionnaires.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a list of active questionnaires that require attention from the user.
    /// </returns>
    /// <remarks>
    /// This method returns questionnaires that are active and require action from
    /// the specified user. The results are sorted by priority and deadline.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when the userId is invalid.</exception>
    public async Task<List<ActiveQuestionnaireBase>> GetPendingActiveQuestionnaires(Guid userId)
    {
        return await _unitOfWork.ActiveQuestionnaire.GetPendingActiveQuestionnaires(userId);
    }
}
