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
using Database.Models;

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
            request.ActiveQuestionnaireId,
            onlyStudentCompleted: request.FilterStudentCompleted,
            onlyTeacherCompleted: request.FilterTeacherCompleted
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

    public async Task<QuestionnaireGroupResult> ActivateQuestionnaireGroup(ActivateQuestionnaireGroup request)
    {
        _ldap.Authenticate(_ldapSettings.SA, _ldapSettings.SAPassword);

        if (!_ldap.connection.Bound) throw new Exception("Failed to bind to the LDAP server.");
        // Ensure all students exist
        foreach (var studentId in request.StudentIds)
        {
            if (!_unitOfWork.User.UserExists(studentId))
            {
                UserAdd student = GenerateStudent(studentId);
                await _unitOfWork.User.AddStudentAsync(student);
            }
        }

        // Ensure all teachers exist
        foreach (var teacherId in request.TeacherIds)
        {
            if (!_unitOfWork.User.UserExists(teacherId))
            {
                UserAdd teacher = GenerateTeacher(teacherId);
                await _unitOfWork.User.AddTeacherAsync(teacher);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        // 1. Create the group
        var group = new QuestionnaireGroupModel
        {
            GroupId = Guid.NewGuid(),
            TemplateId = request.TemplateId,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.QuestionnaireGroup.AddAsync(group);

        // 2. Create questionnaires for each student/teacher
        var createdQuestionnaires = new List<ActiveQuestionnaire>();
        foreach (var studentId in request.StudentIds)
        {
            foreach (var teacherId in request.TeacherIds)
            {
                var questionnaire = await _unitOfWork.ActiveQuestionnaire.ActivateQuestionnaireAsync(
                    request.TemplateId, studentId, teacherId, group.GroupId);
                createdQuestionnaires.Add(questionnaire);
            }
        }
        await _unitOfWork.SaveChangesAsync();

        // 3. Map to DTOs
        var questionnaireDtos = createdQuestionnaires.Select(q => new ActiveQuestionnaireAdminBase
        {
            Id = q.Id,
            Title = q.Title,
            Description = q.Description,
            ActivatedAt = q.ActivatedAt,
            Student = q.Student, // Map to UserBase as needed
            Teacher = q.Teacher,
            StudentCompletedAt = q.StudentCompletedAt,
            TeacherCompletedAt = q.TeacherCompletedAt
        }).ToList();

        // 4. Return group result
        return new QuestionnaireGroupResult
        {
            GroupId = group.GroupId,
            Name = group.Name,
            TemplateId = group.TemplateId,
            Questionnaires = questionnaireDtos
        };
    }

    public async Task<QuestionnaireGroupKeysetPaginationResult> FetchQuestionnaireGroups(QuestionnaireGroupKeysetPaginationRequest request)
    {
        DateTime? cursorCreatedAt = null;
        Guid? cursorId = null;

        if (!string.IsNullOrEmpty(request.QueryCursor))
        {
            cursorCreatedAt = DateTime.Parse(request.QueryCursor.Split('_')[0]);
            cursorId = Guid.Parse(request.QueryCursor.Split('_')[1]);
        }

        (List<QuestionnaireGroupModel> groups, int totalCount) = await _unitOfWork.QuestionnaireGroup
            .PaginationQueryWithKeyset(
                request.PageSize,
                request.Order,
                cursorId,
                cursorCreatedAt,
                request.Title,
                request.GroupId
            );

        var results = groups.Select(group => new QuestionnaireGroupResult
        {
            GroupId = group.GroupId,
            Name = group.Name,
            TemplateId = group.TemplateId,
            Questionnaires = group.Questionnaires.Select(q => new ActiveQuestionnaireAdminBase
            {
                Id = q.Id,
                Title = q.Title,
                Description = q.Description,
                ActivatedAt = q.ActivatedAt,
                Student = new UserBase
                {
                    UserName = q.Student.UserName,
                    FullName = q.Student.FullName
                },
                Teacher = new UserBase
                {
                    UserName = q.Teacher.UserName,
                    FullName = q.Teacher.FullName
                },
                StudentCompletedAt = q.StudentCompletedAt,
                TeacherCompletedAt = q.TeacherCompletedAt
            }).ToList()
        }).ToList();

        QuestionnaireGroupModel? lastGroup = groups.Count > 0 ? groups.Last() : null;

        string? queryCursor = null;
        if (lastGroup is not null)
        {
            queryCursor = $"{lastGroup.CreatedAt:O}_{lastGroup.GroupId}";
        }

        return new QuestionnaireGroupKeysetPaginationResult
        {
            Groups = results, //  now includes questionnaires
            QueryCursor = queryCursor,
            TotalCount = totalCount
        };
    }




    public async Task<QuestionnaireGroupResult?> GetQuestionnaireGroup(Guid groupId)
    {
        // Fetch group from repository
        var group = await _unitOfWork.QuestionnaireGroup.GetByIdAsync(groupId);
        if (group == null)
            return null;

        // Fetch questionnaires for this group
        var questionnaires = group.Questionnaires ?? new List<ActiveQuestionnaireModel>();
        // Fix for CS0029: Explicitly map TeacherModel to UserBase
        var questionnaireDtos = questionnaires.Select(q => new ActiveQuestionnaireAdminBase
        {
            Id = q.Id,
            Title = q.Title,
            Description = q.Description,
            ActivatedAt = q.ActivatedAt,
            Student = new UserBase
            {
                UserName = q.Student.UserName,
                FullName = q.Student.FullName
            },
            Teacher = new UserBase
            {
                UserName = q.Teacher.UserName,
                FullName = q.Teacher.FullName
            },
            StudentCompletedAt = q.StudentCompletedAt,
            TeacherCompletedAt = q.TeacherCompletedAt
        }).ToList();

        return new QuestionnaireGroupResult
        {
            GroupId = group.GroupId,
            Name = group.Name,
            TemplateId = group.TemplateId,
            Questionnaires = questionnaireDtos
        };
    }

    public async Task<List<QuestionnaireGroupResult>> GetAllQuestionnaireGroups()
    {
        var groups = await _unitOfWork.QuestionnaireGroup.GetAllAsync();
        var results = new List<QuestionnaireGroupResult>();

        foreach (var group in groups)
        {
            var questionnaires = group.Questionnaires
                .Select(q => new ActiveQuestionnaireAdminBase
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    ActivatedAt = q.ActivatedAt,
                    Student = new UserBase
                    {
                        UserName = q.Student.UserName,
                        FullName = q.Student.FullName
                    },
                    Teacher = new UserBase
                    {
                        UserName = q.Teacher.UserName,
                        FullName = q.Teacher.FullName
                    },
                    StudentCompletedAt = q.StudentCompletedAt,
                    TeacherCompletedAt = q.TeacherCompletedAt
                }).ToList();

            results.Add(new QuestionnaireGroupResult
            {
                GroupId = group.GroupId,
                Name = group.Name,
                TemplateId = group.TemplateId,
                Questionnaires = questionnaires
            });
        }

        return results;
    }
    public async Task<ActiveQuestionnaire> FetchActiveQuestionnaire(Guid id)
    {
        return await _unitOfWork.ActiveQuestionnaire.GetFullActiveQuestionnaireAsync(id);
    }

    public async Task<List<ActiveQuestionnaire>> ActivateTemplate(ActivateQuestionnaire request)
    {
        _ldap.Authenticate(_ldapSettings.SA, _ldapSettings.SAPassword);

        if (!_ldap.connection.Bound) throw new Exception("Failed to bind to the LDAP server.");

        var createdQuestionnaires = new List<ActiveQuestionnaire>();

        foreach (var studentId in request.StudentIds)
        {
            if (!_unitOfWork.User.UserExists(studentId))
            {
                UserAdd student = GenerateStudent(studentId);
                await _unitOfWork.User.AddStudentAsync(student);
            }

            foreach (var teacherId in request.TeacherIds)
            {
                if (!_unitOfWork.User.UserExists(teacherId))
                {
                    UserAdd teacher = GenerateTeacher(teacherId);
                    await _unitOfWork.User.AddTeacherAsync(teacher);
                }

                // Fix: Add a groupId parameter to the ActivateQuestionnaireAsync call
                var groupId = Guid.NewGuid(); // Generate a new groupId or use an existing one if applicable
                var activeQuestionnaire = await _unitOfWork.ActiveQuestionnaire.ActivateQuestionnaireAsync(
                    request.TemplateId, studentId, teacherId, groupId);

                createdQuestionnaires.Add(activeQuestionnaire);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return createdQuestionnaires;
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

    public async Task<bool> HasUserSubmittedAnswer(Guid userId, Guid activeQuestionnaireId)
    {
        return await _unitOfWork.ActiveQuestionnaire.HasUserSubmittedAnswer(userId, activeQuestionnaireId);
    }

    public async Task<bool> IsActiveQuestionnaireComplete(Guid activeQuestionnaireId, Guid? userId = null)
    {
        if (userId.HasValue)
        {
            return await _unitOfWork.ActiveQuestionnaire.IsActiveQuestionnaireComplete(activeQuestionnaireId, (Guid)userId);
        }
        else
        {
            return await _unitOfWork.ActiveQuestionnaire.IsActiveQuestionnaireComplete(activeQuestionnaireId);
        }
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
// Add the missing CreatedAt property to the QuestionnaireGroupResult class
