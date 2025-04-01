using API.DTO.LDAP;
using API.DTO.Requests.ActiveQuestionnaire;
using API.DTO.Requests.User;
using API.DTO.Responses.ActiveQuestionnaire;
using API.DTO.Responses.User;
using API.Extensions;
using API.Interfaces;
using Database.DTO.ActiveQuestionnaire;

namespace API.Services;

public class UserService(LdapService ldapService, IUnitOfWork unitOfWork)
{
    private readonly LdapService _ldapService = ldapService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public UserQueryPaginationResult QueryLDAPUsersWithPagination(UserQueryPagination request)
    {
        (List<BasicUserInfoWithObjectGuid> ldapUsers, string sessionId, bool hasMore) = _ldapService.SearchUserPagination<BasicUserInfoWithObjectGuid>(request.User, request.Role.ToString(), request.PageSize, request.SessionId);

        return new()
        {
            UserBases = [.. ldapUsers.Select(u => u.ToUserBaseDto())],
            SessionId = sessionId,
            HasMore = hasMore
        };
    }

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

    public async Task<List<ActiveQuestionnaireBase>> GetPendingActiveQuestionnaires(Guid userId)
    {
        return await _unitOfWork.ActiveQuestionnaire.GetPendingActiveQuestionnaires(userId);
    }
}
