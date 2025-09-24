using API.DTO.LDAP;
using API.DTO.Requests.ActiveQuestionnaire;
using API.DTO.Requests.User;
using API.DTO.Responses.ActiveQuestionnaire;
using API.DTO.Responses.User;
using API.Extensions;
using API.Interfaces;
using Database.DTO;
using Database.DTO.ActiveQuestionnaire;
using Database.DTO.User;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
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

    // for search student list from individual class
    //Get all students in a specific group
    public List<LdapUserDTO> GetStudentsInGroup(string groupName)
    {
        return _ldapService.GetStudentsInGroup(groupName);
    }
    // Group students by class
    public List<ClassStudentsDTO> GetStudentsGrouped(List<LdapUserDTO> students)
    {
        return students
            .GroupBy(s => s.ClassName) // group by raw DN
            .Select(g => new ClassStudentsDTO
            {
                ClassName = ParseClassName(g.Key), // optional helper to strip "CN=" etc.
                Students = g.Select(s => s.Name).ToList()
            })
            .ToList();
    }

    // Optional helper to extract just "H1" from "CN=H1,OU=Groups,DC=school,DC=local"
    private string ParseClassName(string distinguishedName)
    {
        if (string.IsNullOrEmpty(distinguishedName)) return string.Empty;
        var firstPart = distinguishedName.Split(',')[0]; // "CN=H1"
        return firstPart.StartsWith("CN=") ? firstPart.Substring(3) : firstPart;
    }
    // Fetch all students from all groups
    //public List<LdapUserDTO> GetAllStudents()
    //{
    //    var allGroups = _ldapService.GetAllGroups(); // You'll implement this in LdapService
    //    var allStudents = new List<LdapUserDTO>();

    //    foreach (var group in allGroups)
    //    {
    //        var students = _ldapService.GetStudentsInGroup(group);
    //        allStudents.AddRange(students);
    //    }

    //    return allStudents;
    //}

    //public List<string> GetAllClassNames()
    //{
    //    var allGroups = _ldapService.GetAllGroups();

    //    // Extract clean class names
    //    return allGroups
    //        .Select(ParseClassName)
    //        .Distinct()
    //        .OrderBy(c => c)
    //        .ToList();
    //}
    public List<string> GetClassesWithStudentRole()
    {
        var result = new List<string> { "Student" }; // Always include "Student"

        // Get all class names from LDAP
        var allGroups = _ldapService.GetAllGroups(); // ["H1", "H2", ...]

        // Optional: filter out non-class groups if needed
        var classGroups = allGroups
            .Where(g => g.StartsWith("H")) // assuming class names start with "H"
            .OrderBy(g => g)
            .ToList();

        result.AddRange(classGroups);

        return result;
    }



}
