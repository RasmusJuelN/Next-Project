using System.Net;
using API.DTO.LDAP;
using API.DTO.Requests.ActiveQuestionnaire;
using API.DTO.Responses.ActiveQuestionnaire;
using API.Exceptions;
using API.Interfaces;
using Database.DTO.ActiveQuestionnaire;
using Database.DTO.User;
using Database.Enums;
using Settings.Models;
using Database.DTO.User;
using API.Exceptions;
using System.Net;
using Database.Models;

namespace API.Services;

/// <summary>
/// Provides business logic and orchestration for active questionnaire operations.
/// Handles the lifecycle management of questionnaires including activation, deactivation,
/// response collection, and retrieval with advanced filtering and pagination capabilities.
/// </summary>
/// <remarks>
/// This service coordinates between the data layer (Unit of Work) and external authentication
/// systems to provide comprehensive questionnaire management functionality. It supports
/// role-based access control and integrates with LDAP for user verification.
/// </remarks>
public class ActiveQuestionnaireService(IUnitOfWork unitOfWork, IAuthenticationBridge authenticationBridge, IConfiguration configuration)
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAuthenticationBridge _authenticationBridge = authenticationBridge;
    private readonly LDAPSettings _ldapSettings = ConfigurationBinderService.Bind<LDAPSettings>(configuration);
    private readonly JWTSettings _JWTSettings = ConfigurationBinderService.Bind<JWTSettings>(configuration);

    /// <summary>
    /// Retrieves a paginated list of active questionnaire base information for administrative purposes.
    /// </summary>
    /// <param name="request">The pagination and filtering parameters for the query.</param>
    /// <returns>
    /// A paginated result containing active questionnaire base data, total count, and navigation metadata.
    /// </returns>
    /// <remarks>
    /// This method supports advanced filtering by title, student, teacher, and completion status.
    /// It uses keyset pagination for efficient navigation through large datasets and maintains
    /// consistent ordering even when data changes during pagination.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when the query cursor format is invalid.</exception>
    /// <exception cref="FormatException">Thrown when cursor date/GUID parsing fails.</exception>
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
        _authenticationBridge.Authenticate(_ldapSettings.SA, _ldapSettings.SAPassword);

        if (!_authenticationBridge.IsConnected()) throw new Exception("Failed to bind to the LDAP server.");
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
                request.GroupId,
                request.PendingStudent,
                request.PendingTeacher
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
    /// <summary>
    /// Fetches an active questionnaire by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the active questionnaire to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the active questionnaire with the specified ID.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the questionnaire with the specified ID is not found.</exception>
    public async Task<ActiveQuestionnaire> FetchActiveQuestionnaire(Guid id)
    {
        return await _unitOfWork.ActiveQuestionnaire.GetFullActiveQuestionnaireAsync(id);
    }

    /// <summary>
    /// Activates a questionnaire template by creating an active questionnaire instance for a specific student and teacher.
    /// Ensures that both the student and teacher exist in the database, creating them if necessary.
    /// </summary>
    /// <param name="request">The activation request containing template ID, student ID, and teacher ID.</param>
    /// <returns>The activated questionnaire instance.</returns>
    /// <exception cref="Exception">Thrown when LDAP authentication fails or connection to LDAP server cannot be established.</exception>
    public async Task<List<ActiveQuestionnaire>> ActivateTemplate(ActivateQuestionnaire request)
    {
        _authenticationBridge.Authenticate(_ldapSettings.SA, _ldapSettings.SAPassword);

        if (!_authenticationBridge.IsConnected()) throw new Exception("Failed to bind to the LDAP server.");

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

    /// <summary>
    /// Retrieves the ID of the oldest active questionnaire assigned to a specific user.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the unique identifier 
    /// of the oldest active questionnaire for the user, or null if no active questionnaires are found.
    /// </returns>
    public async Task<Guid?> GetOldestActiveQuestionnaireForUser(Guid id)
    {
        return await _unitOfWork.User.GetIdOfOldestActiveQuestionnaire(id);
    }

    /// <summary>
    /// Submits user answers for a specific active questionnaire after verifying submission eligibility.
    /// </summary>
    /// <param name="activeQuestionnaireId">The unique identifier of the active questionnaire.</param>
    /// <param name="userId">The unique identifier of the user submitting answers.</param>
    /// <param name="submission">The answer submission data containing user responses.</param>
    /// <returns>A task that represents the asynchronous submission operation.</returns>
    /// <remarks>
    /// This method ensures that users cannot submit multiple responses to the same questionnaire
    /// by checking for existing submissions before processing the new submission.
    /// All answer data is validated and stored atomically in a database transaction.
    /// </remarks>
    /// <exception cref="HttpResponseException">
    /// Thrown with status code 409 (Conflict) when the user has already submitted answers for this questionnaire.
    /// </exception>
    /// <exception cref="ArgumentException">Thrown when the questionnaire ID or user ID is invalid.</exception>
    public async Task SubmitAnswers(Guid activeQuestionnaireId, Guid userId, AnswerSubmission submission)
    {
        if (await _unitOfWork.ActiveQuestionnaire.HasUserSubmittedAnswer(userId, activeQuestionnaireId))
        {
            throw new HttpResponseException(HttpStatusCode.Conflict, "User has already submitted answers for this questionnaire.");
        }

        await _unitOfWork.ActiveQuestionnaire.AddAnswers(activeQuestionnaireId, userId, submission);
        await _unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves the complete response data for a specific active questionnaire.
    /// </summary>
    /// <param name="id">The unique identifier of the active questionnaire.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// the full response data including all submitted answers and metadata.
    /// </returns>
    /// <remarks>
    /// This method provides comprehensive response data suitable for analysis and reporting.
    /// It includes all user submissions, timestamps, and questionnaire context information.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when the questionnaire ID is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the questionnaire is not found.</exception>
    public async Task<FullResponse> GetFullResponseAsync(Guid id)
    {
        return await _unitOfWork.ActiveQuestionnaire.GetFullResponseAsync(id);
    }

    /// <summary>
    /// Checks whether a specific user has already submitted answers for a given active questionnaire.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to check.</param>
    /// <param name="activeQuestionnaireId">The unique identifier of the active questionnaire.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// <c>true</c> if the user has submitted answers; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method is typically used to prevent duplicate submissions.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when the user ID or questionnaire ID is invalid.</exception>
    public async Task<bool> HasUserSubmittedAnswer(Guid userId, Guid activeQuestionnaireId)
    {
        return await _unitOfWork.ActiveQuestionnaire.HasUserSubmittedAnswer(userId, activeQuestionnaireId);
    }

    /// <summary>
    /// Determines whether an active questionnaire is complete based on submission status.
    /// </summary>
    /// <param name="activeQuestionnaireId">The unique identifier of the active questionnaire to check.</param>
    /// <param name="userId">
    /// Optional user identifier to check completion status for a specific user.
    /// If null, checks overall questionnaire completion status.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// <c>true</c> if the questionnaire is complete according to the specified criteria; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method supports two modes of operation:
    /// <list type="bullet">
    /// <item><description>User-specific: Checks if a particular user has completed the questionnaire</description></item>
    /// <item><description>Overall: Checks if the questionnaire meets general completion criteria</description></item>
    /// </list>
    /// Completion criteria may vary based on questionnaire type and business rules.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when the questionnaire ID is invalid.</exception>
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

    internal async Task<List<FullStudentRespondsDate>> GetResponsesFromStudentAndTemplateAsync(Guid studentid, Guid templateid)
    {

        return await _unitOfWork.ActiveQuestionnaire.GetResponsesFromStudentAndTemplateAsync(studentid, templateid);

    }

    internal async Task<List<FullStudentRespondsDate>> GetResponsesFromStudentAndTemplateWithDateAsync(Guid studentid, Guid templateid)
    {

        return await _unitOfWork.ActiveQuestionnaire.GetResponsesFromStudentAndTemplateWithDateAsync(studentid, templateid);

    }

    internal async Task<SurveyResponseSummary> GetAnonymisedResponses(Guid templateId, List<Guid> users, List<Guid> groups)
    {
        return await _unitOfWork.ActiveQuestionnaire.GetAnonymisedResponses(templateId, users, groups);
    }

    /// <summary>
    /// Generates a student user entity from user storage information for database insertion.
    /// </summary>
    /// <param name="id">The unique identifier of the student to retrieve from user storage.</param>
    /// <returns>A <see cref="UserAdd"/> object populated with student information from user storage.</returns>
    /// <remarks>
    /// This private method handles the conversion from user storage data to the application's user model.
    /// It extracts role information from user group membership and maps it to the application's
    /// role and permission system. The method ensures that student records in the local database
    /// are synchronized with the authoritative user storage system.
    /// <para>
    /// Note: The generic constraint limitations prevent making this method generic due to required properties.
    /// </para>
    /// </remarks>
    /// <exception cref="HttpResponseException">
    /// Thrown with status code 404 (Not Found) when the student is not found in the user storage system.
    /// </exception>
    /// <exception cref="ArgumentException">Thrown when the role mapping from user storage groups fails.</exception>
    // The new() constraint on generics don't allow classes with required properties, so we can't make this generic :v
    private UserAdd GenerateStudent(Guid id)
    {
        BasicUserInfo? ldapStudent = _authenticationBridge.SearchId<BasicUserInfo>(id.ToString()) ?? throw new HttpResponseException(HttpStatusCode.NotFound, "Student not found in LDAP.");
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

    /// <summary>
    /// Generates a teacher user entity from user storage information for database insertion.
    /// </summary>
    /// <param name="id">The unique identifier of the teacher to retrieve from user storage.</param>
    /// <returns>A <see cref="UserAdd"/> object populated with teacher information from user storage.</returns>
    /// <remarks>
    /// This private method handles the conversion from user storage data to the application's user model.
    /// It extracts role information from user group membership and maps it to the application's
    /// role and permission system. The method ensures that teacher records in the local database
    /// are synchronized with the authoritative user storage system.
    /// <para>
    /// Note: The generic constraint limitations prevent making this method generic due to required properties.
    /// </para>
    /// </remarks>
    /// <exception cref="HttpResponseException">
    /// Thrown with status code 404 (Not Found) when the teacher is not found in the user storage system.
    /// </exception>
    /// <exception cref="ArgumentException">Thrown when the role mapping from user storage groups fails.</exception>
    private UserAdd GenerateTeacher(Guid id)
    {
        BasicUserInfo? ldapTeacher = _authenticationBridge.SearchId<BasicUserInfo>(id.ToString()) ?? throw new HttpResponseException(HttpStatusCode.NotFound, "Teacher not found in LDAP.");
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
