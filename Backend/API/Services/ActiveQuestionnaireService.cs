using API.DTO.LDAP;
using API.DTO.Requests.ActiveQuestionnaire;
using API.DTO.Responses.ActiveQuestionnaire;
using API.Interfaces;
using Database.DTO.ActiveQuestionnaire;
using Database.Enums;
using Settings.Models;
using Database.DTO.User;
using API.Exceptions;
using System.Net;

namespace API.Services;

public class ActiveQuestionnaireService(IUnitOfWork unitOfWork, LdapService ldap, IConfiguration configuration)
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly LdapService _ldap = ldap;
    private readonly LDAPSettings _ldapSettings = ConfigurationBinderService.Bind<LDAPSettings>(configuration);
    private readonly JWTSettings _JWTSettings = ConfigurationBinderService.Bind<JWTSettings>(configuration);

    public async Task<ActiveQuestionnaireKeysetPaginationResultAdmin> FetchActiveQuestionnaireBases(ActiveQuestionnaireKeysetPaginationRequestFull request)
    {
        DateTime? cursorActivatedAt = null;
        Guid? cursorId = null;

        if (!string.IsNullOrEmpty(request.QueryCursor))
        {
            cursorActivatedAt = DateTime.Parse(request.QueryCursor.Split('_')[0]);
            cursorId = Guid.Parse(request.QueryCursor.Split('_')[1]);
        }

        (List<ActiveQuestionnaireBase> activeQuestionnaireBases, int totalCount) = await _unitOfWork.ActiveQuestionnaire
        .PaginationQueryWithKeyset(
            request.PageSize,
            request.Order,
            cursorId,
            cursorActivatedAt,
            request.Title,
            request.Student,
            request.Teacher,
            request.ActiveQuestionnaireId
        );

        ActiveQuestionnaireBase? lastActiveQuestionnaire = activeQuestionnaireBases.Count != 0 ? activeQuestionnaireBases.Last() : null;

        string? queryCursor = null;
        if (lastActiveQuestionnaire is not null)
        {
            queryCursor = $"{lastActiveQuestionnaire.ActivatedAt:O}_{lastActiveQuestionnaire.Id}";
        }

        return new()
        {
            ActiveQuestionnaireBases = activeQuestionnaireBases,
            QueryCursor = queryCursor,
            TotalCount = totalCount
        };
    }

    public async Task<ActiveQuestionnaire> FetchActiveQuestionnaire(Guid id)
    {
        return await _unitOfWork.ActiveQuestionnaire.GetFullActiveQuestionnaireAsync(id);
    }

    public async Task<ActiveQuestionnaire> ActivateTemplate(ActivateQuestionnaire request)
    {
        _ldap.Authenticate(_ldapSettings.SA, _ldapSettings.SAPassword);

        if (!_ldap.connection.Bound) throw new Exception("Failed to bind to the LDAP server.");

        // If the student or teacher is not found in the database, create and add them to the database
        if (!_unitOfWork.User.UserExists(request.StudentId))
        {
            UserAdd student = GenerateStudent(request.StudentId);
            await _unitOfWork.User.AddStudentAsync(student);
        }

        if (!_unitOfWork.User.UserExists(request.TeacherId))
        {
            UserAdd teacher = GenerateTeacher(request.TeacherId);
            await _unitOfWork.User.AddTeacherAsync(teacher);
        }

        ActiveQuestionnaire activeQuestionnaire = await _unitOfWork.ActiveQuestionnaire.ActivateQuestionnaireAsync(request.TemplateId, request.StudentId, request.TeacherId);
        await _unitOfWork.SaveChangesAsync();

        return activeQuestionnaire;
    }

    public async Task<Guid?> GetOldestActiveQuestionnaireForUser(Guid id)
    {
        return await _unitOfWork.User.GetIdOfOldestActiveQuestionnaire(id);
    }

    public async Task SubmitAnswers(Guid activeQuestionnaireId, Guid userId, AnswerSubmission submission)
    {
        if (await _unitOfWork.ActiveQuestionnaire.HasUserSubmittedAnswer(userId, activeQuestionnaireId))
        {
            throw new HttpResponseException(HttpStatusCode.Conflict, "User has already submitted answers for this questionnaire.");
        }
        
        await _unitOfWork.ActiveQuestionnaire.AddAnswers(activeQuestionnaireId, userId, submission);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<FullResponse> GetFullResponseAsync(Guid id)
    {
        return await _unitOfWork.ActiveQuestionnaire.GetFullResponseAsync(id);
    }

    // The new() constraint on generics don't allow classes with required properties, so we can't make this generic :v
    private UserAdd GenerateStudent(Guid id)
    {
        BasicUserInfo ldapStudent = _ldap.SearchByObjectGUID<BasicUserInfo>(id);
        string studentRole = _JWTSettings.Roles.FirstOrDefault(x => ldapStudent.MemberOf.StringValue.Contains(x.Value)).Key;
        
        return new()
        {
            Guid = id,
            UserName = ldapStudent.Username.StringValue,
            FullName = ldapStudent.Name.StringValue,
            PrimaryRole = (UserRoles)Enum.Parse(typeof(UserRoles), studentRole, true),
            Permissions = (UserPermissions)Enum.Parse(typeof(UserPermissions), studentRole, true)
        };
    }

    private UserAdd GenerateTeacher(Guid id)
    {
        BasicUserInfo ldapTeacher = _ldap.SearchByObjectGUID<BasicUserInfo>(id);
        string teacherRole = _JWTSettings.Roles.FirstOrDefault(x => ldapTeacher.MemberOf.StringValue.Contains(x.Value)).Key;
        
        return new()
        {
            Guid = id,
            UserName = ldapTeacher.Username.StringValue,
            FullName = ldapTeacher.Name.StringValue,
            PrimaryRole = (UserRoles)Enum.Parse(typeof(UserRoles), teacherRole, true),
            Permissions = (UserPermissions)Enum.Parse(typeof(UserPermissions), teacherRole, true)
        };
    }
}
