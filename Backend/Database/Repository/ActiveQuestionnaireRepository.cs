using Database.DTO.ActiveQuestionnaire;
using Database.DTO.QuestionnaireTemplate;
using Database.Enums;
using Database.Extensions;
using Database.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Database.Repository;

/// <summary>
/// Implements repository operations for active questionnaire management.
/// Provides comprehensive functionality for questionnaire lifecycle management including activation, completion tracking, and response handling.
/// </summary>
/// <remarks>
/// This repository manages the complete lifecycle of active questionnaires from template activation through
/// completion by both participants. It handles complex filtering, pagination, and workflow state management
/// while maintaining data integrity and performance through optimized database queries.
/// </remarks>
/// <param name="context">The database context for data access operations.</param>
/// <param name="loggerFactory">Factory for creating loggers for diagnostic and monitoring purposes.</param>
public class ActiveQuestionnaireRepository(Context context, ILoggerFactory loggerFactory) : IActiveQuestionnaireRepository
{
    private readonly Context _context = context;
    private readonly GenericRepository<ActiveQuestionnaireModel> _genericRepository = new(context, loggerFactory);

    /// <summary>
    /// Retrieves basic information about an active questionnaire.
    /// </summary>
    /// <param name="id">The unique identifier of the active questionnaire.</param>
    /// <returns>An ActiveQuestionnaireBase DTO containing essential questionnaire information.</returns>
    /// <exception cref="Exception">Thrown when the active questionnaire with the specified ID is not found.</exception>
    /// <remarks>
    /// This method includes related student and teacher information in the query for efficient data retrieval.
    /// Returns a lightweight DTO suitable for list displays and summary views.
    /// </remarks>
    public async Task<ActiveQuestionnaireBase> GetActiveQuestionnaireBaseAsync(Guid id)
    {
        ActiveQuestionnaireModel activeQuestionnaire = await _genericRepository.GetSingleAsync(a => a.Id == id,
            query => query.Include(a => a.Student).Include(a => a.Teacher)) ?? throw new Exception("Active questionnaire not found.");

        return activeQuestionnaire.ToBaseDto();
    }

    /// <summary>
    /// Retrieves complete information about an active questionnaire including all questions and their options.
    /// </summary>
    /// <param name="id">The unique identifier of the active questionnaire.</param>
    /// <returns>A complete ActiveQuestionnaire DTO with all questionnaire structure and participant information.</returns>
    /// <exception cref="Exception">Thrown when the active questionnaire with the specified ID is not found.</exception>
    /// <remarks>
    /// This method performs a comprehensive query including student, teacher, template questions, and options.
    /// Returns the full questionnaire structure suitable for detailed views and response collection interfaces.
    /// </remarks>
    public async Task<ActiveQuestionnaire> GetFullActiveQuestionnaireAsync(Guid id)
    {
        ActiveQuestionnaireModel activeQuestionnaire = await _genericRepository.GetSingleAsync(a => a.Id == id,
            query => query.Include(a => a.Student).Include(a => a.Teacher).Include(a => a.QuestionnaireTemplate.Questions).ThenInclude(q => q.Options)) ?? throw new Exception("Active questionnaire not found.");

        return activeQuestionnaire.ToDto();
    }

    /// <inheritdoc/>
    public async Task<ActiveQuestionnaire> ActivateQuestionnaireAsync(
        Guid questionnaireTemplateId,
        Guid studentId,
        Guid teacherId,
        Guid groupId,
        ActiveQuestionnaireType activeQuestionnaireType)
    {
        // Fetch student and teacher
        StudentModel student = _context.Users.Local.OfType<StudentModel>().SingleOrDefault(u => u.Guid == studentId) ?? await _context.Users.OfType<StudentModel>().SingleAsync(u => u.Guid == studentId);
        TeacherModel teacher = _context.Users.Local.OfType<TeacherModel>().SingleOrDefault(u => u.Guid == teacherId) ?? await _context.Users.OfType<TeacherModel>().SingleAsync(u => u.Guid == teacherId);

        // Fetch template
        QuestionnaireTemplateModel questionnaireTemplate = await _context.QuestionnaireTemplates
            .Include(t => t.Questions)
            .ThenInclude(q => q.Options)
            .SingleAsync(t => t.Id == questionnaireTemplateId);

        // Create questionnaire and set groupId
        ActiveQuestionnaireModel activeQuestionnaire = new()
        {
            Title = questionnaireTemplate.Title,
            Description = questionnaireTemplate.Description,
            Student = student,
            Teacher = teacher,
            QuestionnaireTemplate = questionnaireTemplate,
            GroupId = groupId,
            QuestionnaireType = activeQuestionnaireType
        };

        await _genericRepository.AddAsync(activeQuestionnaire);

        return activeQuestionnaire.ToDto();
    }

    /// <inheritdoc/>
    public async Task<(List<ActiveQuestionnaireBase>, int)> PaginationQueryWithKeyset(
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
        ActiveQuestionnaireType? questionnaireType = null)
    {
        IQueryable<ActiveQuestionnaireModel> query = _genericRepository.GetAsQueryable();

        query = sortOrder.ApplyQueryOrdering(query);
        if (!string.IsNullOrEmpty(titleQuery))
        {
            query = query.Where(q => q.Title.Contains(titleQuery));
        }

        if (idQuery is not null)
        {
            query = query.Where(q => q.Id.ToString().Contains(idQuery.ToString()!));
        }

        if (!string.IsNullOrEmpty(student))
        {
            query = query.Where(q => q.Student.FullName.Contains(student) || q.Student.UserName.Contains(student));
        }

        if (!string.IsNullOrEmpty(teacher))
        {
            query = query.Where(q => q.Teacher.FullName.Contains(teacher) || q.Teacher.UserName.Contains(teacher));
        }

        if (userId.HasValue)
        {
            query = query.Where(q => q.Student.Guid == userId || q.Teacher.Guid == userId);
        }

        if (onlyStudentCompleted)
        {
            query = query.Where(q => q.StudentCompletedAt.HasValue);
        }

        if (onlyTeacherCompleted)
        {
            query = query.Where(q => q.TeacherCompletedAt.HasValue);
        }

        if (pendingStudent)
            query = query.Where(q => !q.StudentCompletedAt.HasValue);

        if (pendingTeacher)
            query = query.Where(q => !q.TeacherCompletedAt.HasValue);
        
        if (questionnaireType is not null)
        {
            query = query.Where(q => q.QuestionnaireType == questionnaireType);
        }

        int totalCount = await query.CountAsync();

        if (cursorIdPosition is not null && cursorActivatedAtPosition is not null)
        {
            if (sortOrder == ActiveQuestionnaireOrderingOptions.ActivatedAtAsc)
            {
                query = query.Where(q => q.ActivatedAt > cursorActivatedAtPosition
                || q.ActivatedAt == cursorActivatedAtPosition && q.Id > cursorIdPosition);
            }
            else
            {
                query = query.Where(q => q.ActivatedAt < cursorActivatedAtPosition
                || q.ActivatedAt == cursorActivatedAtPosition && q.Id < cursorIdPosition);
            }
        }

        List<ActiveQuestionnaireModel> questionnaireTemplates = await query.Include(q => q.Student).Include(q => q.Teacher).Take(amount).ToListAsync();
        List<ActiveQuestionnaireBase> questionnaireTemplateBases = [.. questionnaireTemplates.Select(t => t.ToBaseDto())];

        return (questionnaireTemplateBases, totalCount);
    }



    /// <summary>
    /// Submits responses for a specific active questionnaire on behalf of a user.
    /// </summary>
    /// <param name="activeQuestionnaireId">The ID of the active questionnaire to submit responses for.</param>
    /// <param name="userId">The GUID of the user submitting the responses.</param>
    /// <param name="submission">The answer submission containing responses to questionnaire questions.</param>
    /// <exception cref="InvalidOperationException">Thrown when the questionnaire or user is not found.</exception>
    /// <remarks>
    /// This method handles response submission for both students and teachers, creating appropriate response records
    /// based on user type. Automatically sets completion timestamps when responses are submitted.
    /// Student and teacher responses are stored separately to maintain workflow independence.
    /// </remarks>
    public async Task AddAnswers(Guid activeQuestionnaireId, Guid userId, AnswerSubmission submission)
    {
        ActiveQuestionnaireModel activeQuestionnaire = await _context.ActiveQuestionnaires.FirstAsync(a => a.Id == activeQuestionnaireId);
        UserBaseModel user = await _context.Users.OfType<UserBaseModel>().SingleAsync(u => u.Guid == userId);

        if (user.GetType().Equals(typeof(StudentModel)))
        {
            foreach (Answer answer in submission.Answers)
            {
                ActiveQuestionnaireStudentResponseModel response = new()
                {
                    QuestionFK = answer.QuestionId,
                    OptionFK = answer.OptionId,
                    CustomResponse = answer.CustomAnswer,
                };
                activeQuestionnaire.StudentAnswers.Add(response);
            }
            activeQuestionnaire.StudentCompletedAt = DateTime.UtcNow;
        }
        else if (user.GetType().Equals(typeof(TeacherModel)))
        {
            foreach (Answer answer in submission.Answers)
            {
                ActiveQuestionnaireTeacherResponseModel response = new()
                {
                    QuestionFK = answer.QuestionId,
                    OptionFK = answer.OptionId,
                    CustomResponse = answer.CustomAnswer,
                };
                activeQuestionnaire.TeacherAnswers.Add(response);
            }
            activeQuestionnaire.TeacherCompletedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Checks if a specific user has submitted responses for an active questionnaire.
    /// </summary>
    /// <param name="userId">The GUID of the user to check.</param>
    /// <param name="activeQuestionnaireId">The ID of the active questionnaire.</param>
    /// <returns>True if the user has submitted responses, false otherwise.</returns>
    /// <exception cref="Exception">Thrown when the user is not found or is neither a student nor teacher.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the questionnaire is not found.</exception>
    /// <remarks>
    /// This method determines completion status based on user type, checking the appropriate completion timestamp.
    /// Used to prevent duplicate submissions and manage workflow progression.
    /// </remarks>
    public async Task<bool> HasUserSubmittedAnswer(Guid userId, Guid activeQuestionnaireId)
    {
        UserBaseModel user = await _context.Users.SingleAsync(u => u.Guid == userId);
        ActiveQuestionnaireModel activeQuestionnaire = await _context.ActiveQuestionnaires.SingleAsync(a => a.Id == activeQuestionnaireId);

        if (user.GetType().Equals(typeof(StudentModel)))
        {
            return activeQuestionnaire.StudentCompletedAt is not null;
        }
        else if (user.GetType().Equals(typeof(TeacherModel)))
        {
            return activeQuestionnaire.TeacherCompletedAt is not null;
        }
        else
        {
            throw new Exception("User is not a student or teacher.");
        }
    }

    /// <summary>
    /// Checks if an active questionnaire is completely finished by both participants.
    /// </summary>
    /// <param name="activeQuestionnaireId">The ID of the active questionnaire to check.</param>
    /// <returns>True if both student and teacher have completed their portions, false otherwise.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the questionnaire is not found.</exception>
    /// <remarks>
    /// A questionnaire is considered complete when both participants have submitted their responses
    /// and their respective completion timestamps are set. Used for workflow management and reporting.
    /// </remarks>
    public async Task<bool> IsActiveQuestionnaireComplete(Guid activeQuestionnaireId)
    {
        ActiveQuestionnaireModel activeQuestionnaire = await _context.ActiveQuestionnaires.SingleAsync(a => a.Id == activeQuestionnaireId);

        return activeQuestionnaire.StudentCompletedAt.HasValue && activeQuestionnaire.TeacherCompletedAt.HasValue;
    }

    /// <summary>
    /// Checks if an active questionnaire is completely finished, with verification that the user is a participant.
    /// </summary>
    /// <param name="activeQuestionnaireId">The ID of the active questionnaire to check.</param>
    /// <param name="userId">The GUID of the user to verify as a participant.</param>
    /// <returns>True if both participants have completed their portions, false otherwise.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the questionnaire is not found or the user is not a participant.</exception>
    /// <remarks>
    /// This method combines completion checking with participant verification to ensure the requesting user
    /// has permission to view the completion status. The user must be either the assigned student or teacher.
    /// </remarks>
    public async Task<bool> IsActiveQuestionnaireComplete(Guid activeQuestionnaireId, Guid userId)
    {
        ActiveQuestionnaireModel activeQuestionnaire = await _context.ActiveQuestionnaires.SingleAsync(a => a.Id == activeQuestionnaireId && (a.Student.Guid == userId || a.Teacher.Guid == userId));

        return activeQuestionnaire.StudentCompletedAt.HasValue && activeQuestionnaire.TeacherCompletedAt.HasValue;
    }

    /// <summary>
    /// Retrieves the complete response data for an active questionnaire including all submitted responses.
    /// </summary>
    /// <param name="id">The ID of the active questionnaire.</param>
    /// <returns>A FullResponse DTO containing all questionnaire data and submitted responses from both participants.</returns>
    /// <exception cref="Exception">Thrown when the questionnaire is not found or not yet completed by both participants.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the questionnaire is not found.</exception>
    /// <remarks>
    /// This method requires both participants to have completed their responses before allowing access to the full data.
    /// Includes comprehensive response information with questions, options, and custom answers for analysis and reporting.
    /// Performs complex include operations to load all related data efficiently in a single query.
    /// </remarks>
    public async Task<FullResponse> GetFullResponseAsync(Guid id)
    {
        ActiveQuestionnaireModel activeQuestionnaire = await _context.ActiveQuestionnaires
            .Include(a => a.StudentAnswers)
            .ThenInclude(a => a.Question)
            .ThenInclude(q => q.Options)
            .Include(a => a.StudentAnswers)
            .ThenInclude(a => a.Option)
            .Include(a => a.TeacherAnswers)
            .ThenInclude(a => a.Question)
            .ThenInclude(q => q.Options)
            .Include(a => a.TeacherAnswers)
            .ThenInclude(a => a.Option)
            .Include(a => a.Student)
            .Include(a => a.Teacher)
            .SingleAsync(a => a.Id == id);

        if (!activeQuestionnaire.StudentCompletedAt.HasValue || !activeQuestionnaire.TeacherCompletedAt.HasValue)
        {
            throw new Exception("The requested Active Questionnaire is not yet completed.");
        }

        return activeQuestionnaire.ToFullResponseAll();
    }


    /// <summary>
    /// Retrieves all pending active questionnaires for a specific user.
    /// </summary>
    /// <param name="userId">The GUID of the user to get pending questionnaires for.</param>
    /// <returns>A list of ActiveQuestionnaireBase DTOs representing questionnaires awaiting completion.</returns>
    /// <exception cref="Exception">Thrown when the user is not found or is neither a student nor teacher.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the user is not found.</exception>
    /// <remarks>
    /// Pending questionnaires are those where the user has not yet completed their portion of the responses.
    /// Used to display user dashboards and task lists showing work that needs to be completed.
    /// </remarks>
    public async Task<List<ActiveQuestionnaireBase>> GetPendingActiveQuestionnaires(Guid userId)
    {
        UserBaseModel user = await _context.Users.SingleOrDefaultAsync(u => u.Guid == userId) ?? throw new InvalidOperationException("User not found.");
        List<ActiveQuestionnaireModel> activeQuestionnaireBases;
        if (user.GetType().Equals(typeof(StudentModel)))
        {
            activeQuestionnaireBases = await _context.ActiveQuestionnaires.Include(a => a.Teacher).Where(a => a.Student.Guid == userId && !a.StudentCompletedAt.HasValue).ToListAsync();
        }
        else if (user.GetType().Equals(typeof(TeacherModel)))
        {
            activeQuestionnaireBases = await _context.ActiveQuestionnaires.Include(a => a.Student).Where(a => a.Teacher.Guid == userId && !a.TeacherCompletedAt.HasValue).ToListAsync();
        }
        else
        {
            throw new Exception("User is not a student or teacher.");
        }



        return [.. activeQuestionnaireBases.Select(a => a.ToBaseDto())];

    }


    public async Task<List<FullStudentRespondsDate>> GetResponsesFromStudentAndTemplateAsync(Guid studentid, Guid templateid)
    {
        //get template based on templateid
        QuestionnaireTemplateModel template = await _context.QuestionnaireTemplates.SingleAsync(t => t.Id == templateid);
        if (template.Title.IsNullOrEmpty())
        {
            throw new Exception("The requested Template Questionnaire does not exsist.");
        }


        //get all activae questionares where it's id is equal to templateId AND studentid
        List<ActiveQuestionnaireModel> activeQuestionnaires = await _context.ActiveQuestionnaires
            .Include(a => a.StudentAnswers)
            .ThenInclude(a => a.Question)
            .Include(a => a.StudentAnswers)
            .ThenInclude(a => a.Option)
            .Include(a => a.Student)
            .Where(a => a.Student.Guid == studentid && a.QuestionnaireTemplate.Id == templateid && a.StudentCompletedAt.HasValue)
            .ToListAsync();

        return [.. activeQuestionnaires.Select(a => a.ToFullStudentRespondsDate())];
    }

    public async Task<List<FullStudentRespondsDate>> GetResponsesFromStudentAndTemplateWithDateAsync(Guid studentid, Guid templateid)
    {
        QuestionnaireTemplateModel template = await _context.QuestionnaireTemplates.SingleAsync(t => t.Id == templateid);
        if (template.Title.IsNullOrEmpty())
        {
            throw new Exception("The requested Template Questionnaire does not exsist.");
        }

        List<ActiveQuestionnaireModel> activeQuestionnaires = await _context.ActiveQuestionnaires
            .Include(a => a.StudentAnswers)
            .ThenInclude(a => a.Question)
            .Include(a => a.StudentAnswers)
            .ThenInclude(a => a.Option)
            .Include(a => a.TeacherAnswers)
            .ThenInclude(a => a.Question)
            .Include(a => a.TeacherAnswers)
            .ThenInclude(a => a.Option)
            .Include(a => a.Student)
            .Include(a => a.Teacher)
            .Where(a => a.Student.Guid == studentid && a.QuestionnaireTemplate.Id == templateid && (a.StudentCompletedAt.HasValue ||  a.TeacherCompletedAt.HasValue))
            .ToListAsync();

        return [.. activeQuestionnaires.Select(a => a.ToFullStudentRespondsDate())];
    }

    public async Task<SurveyResponseSummary> GetAnonymisedResponses(Guid templateId, List<Guid> users, List<Guid> groups)
    {
        // Get the template with its active questionnaires and all related data
        var templateData = await _context.QuestionnaireTemplates
            .Where(t => t.Id == templateId)
            .Select(t => new
            {
                t.Title,
                t.Description,
                ActiveQuestionnaires = t.ActiveQuestionnaires
                    .Where(a => 
                        // Filter by groups if provided
                        groups.Count == 0 || groups.Contains(a.GroupId) &&
                        a.QuestionnaireType == ActiveQuestionnaireType.Anonymous
                    )
                    .Select(a => new
                    {
                        a.Id,
                        ActivatedAt = a.ActivatedAt.Date,
                        StudentAnswers = a.StudentAnswers
                            .Where(sa => users.Count == 0 || (a.Student != null && users.Contains(a.Student.Guid)))
                            .Select(sa => new
                            {
                                QuestionPrompt = sa.Question!.Prompt,
                                Answer = sa.CustomResponse ?? sa.Option!.DisplayText
                            }).ToList(),
                        TeacherAnswers = a.TeacherAnswers
                            .Where(ta => users.Count == 0 || (a.Teacher != null && users.Contains(a.Teacher.Guid)))
                            .Select(ta => new
                            {
                                QuestionPrompt = ta.Question!.Prompt,
                                Answer = ta.CustomResponse ?? ta.Option!.DisplayText
                            }).ToList()
                    }).ToList()
            })
            .SingleOrDefaultAsync() ?? throw new Exception("Survey response summary not found.");

        // Create individual results for each questionnaire (one dataset per questionnaire)
        var anonymisedDataSet = templateData.ActiveQuestionnaires.Select(questionnaire => 
        {
            var allAnswers = new List<dynamic>();
            allAnswers.AddRange(questionnaire.StudentAnswers);
            allAnswers.AddRange(questionnaire.TeacherAnswers);

            return new AnonymisedSurveyResults
            {
                DatasetTitle = questionnaire.ActivatedAt.ToString("yyyy-MM-dd"),
                ParticipantCount = questionnaire.StudentAnswers.Count + questionnaire.TeacherAnswers.Count,
                AnonymisedResponses = [.. allAnswers
                    .GroupBy(response => response.QuestionPrompt)
                    .Select(questionGroup => new AnonymisedResponsesQuestion
                    {
                        Question = questionGroup.Key,
                        Answers = [.. questionGroup
                            .GroupBy(response => response.Answer)
                            .Select(answerGroup => new AnonymisedResponsesAnswer
                            {
                                Answer = answerGroup.Key,
                                Count = answerGroup.Count()
                            })]
                    })]
            };
        })
        .OrderBy(result => result.DatasetTitle)
        .ToList();

        return new SurveyResponseSummary
        {
            Title = templateData.Title,
            Description = templateData.Description,
            AnonymisedResponseDataSet = anonymisedDataSet
        };
    }

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
    public async Task<StudentResultHistory?> GetResponseHistoryAsync(Guid studentId, Guid teacherId, Guid templateId)
    {
        // Get all active questionnaires for the specific student, teacher, and template combination
        var questionnaires = await _context.ActiveQuestionnaires
            .Include(aq => aq.Student)
            .Include(aq => aq.Teacher)
            .Include(aq => aq.QuestionnaireTemplate)
                .ThenInclude(qt => qt!.Questions)
                    .ThenInclude(q => q.Options)
            .Include(aq => aq.StudentAnswers)
                .ThenInclude(sa => sa.Question)
            .Include(aq => aq.StudentAnswers)
                .ThenInclude(sa => sa.Option)
            .Include(aq => aq.TeacherAnswers)
                .ThenInclude(ta => ta.Question)
            .Include(aq => aq.TeacherAnswers)
                .ThenInclude(ta => ta.Option)
            .Where(aq => aq.Student!.Guid == studentId && 
                         aq.Teacher!.Guid == teacherId && 
                         aq.QuestionnaireTemplateFK == templateId &&
                         (aq.StudentCompletedAt != null || aq.TeacherCompletedAt != null)) // Only include questionnaires with at least one completion
            .OrderBy(aq => aq.ActivatedAt)
            .ToListAsync();

        if (!questionnaires.Any())
            return null;

        var firstQuestionnaire = questionnaires.First();
        
        return new StudentResultHistory
        {
            Student = new DTO.User.UserBase
            {
                UserName = firstQuestionnaire.Student!.UserName,
                FullName = firstQuestionnaire.Student.FullName
            },
            Teacher = new DTO.User.UserBase
            {
                UserName = firstQuestionnaire.Teacher!.UserName,
                FullName = firstQuestionnaire.Teacher.FullName
            },
            Template = new QuestionnaireTemplate
            {
                Id = firstQuestionnaire.QuestionnaireTemplate!.Id,
                Title = firstQuestionnaire.QuestionnaireTemplate.Title,
                Description = firstQuestionnaire.QuestionnaireTemplate.Description,
                CreatedAt = firstQuestionnaire.QuestionnaireTemplate.CreatedAt,
                LastUpdated = firstQuestionnaire.QuestionnaireTemplate.LastUpated,
                IsLocked = firstQuestionnaire.QuestionnaireTemplate.IsLocked,
                Questions = firstQuestionnaire.QuestionnaireTemplate.Questions.OrderBy(q => q.SortOrder).Select(q => new QuestionnaireTemplateQuestion
                {
                    Id = q.Id,
                    Prompt = q.Prompt,
                    AllowCustom = q.AllowCustom,
                    SortOrder = q.SortOrder,
                    Options = q.Options.OrderBy(o => o.SortOrder).Select(o => new QuestionnaireTemplateOption
                    {
                        Id = o.Id,
                        DisplayText = o.DisplayText,
                        OptionValue = o.OptionValue,
                        SortOrder = o.SortOrder
                    }).ToList()
                }).ToList()
            },
            AnswersInfo = questionnaires.Select(aq => new AnswerInfo
            {
                activeQuestionnaireId = aq.Id,
                StudentCompletedAt = aq.StudentCompletedAt!,
                TeacherCompletedAt = aq.TeacherCompletedAt!,

                Answers = aq.QuestionnaireTemplate!.Questions.Select(q =>
                {
                    var studentAnswer = aq.StudentAnswers.FirstOrDefault(sa => sa.QuestionFK == q.Id);
                    var teacherAnswer = aq.TeacherAnswers.FirstOrDefault(ta => ta.QuestionFK == q.Id);

                    return new AnswerDetails
                    {
                        QuestionId = q.Id.ToString(),
                        StudentResponse = studentAnswer?.CustomResponse,
                        IsStudentResponseCustom = !string.IsNullOrEmpty(studentAnswer?.CustomResponse),
                        SelectedOptionIdsByStudent = studentAnswer?.OptionFK.HasValue == true ? [studentAnswer.OptionFK.Value] : null,
                        TeacherResponse = teacherAnswer?.CustomResponse,
                        IsTeacherResponseCustom = !string.IsNullOrEmpty(teacherAnswer?.CustomResponse),
                        SelectedOptionIdsByTeacher = teacherAnswer?.OptionFK.HasValue == true ? [teacherAnswer.OptionFK.Value] : null
                    };
                }).ToList()
            }).ToList()
        };


    }

    /// <summary>
    /// Gets all completed active questionnaires in the same group
    /// </summary>
    public async Task<List<ActiveQuestionnaireBase>> GetCompletedQuestionnairesByGroupAsync(Guid activeQuestionnaireId)
    {
        var sourceQuestionnaire = await _context.Set<ActiveQuestionnaireModel>()
            .Where(aq => aq.Id == activeQuestionnaireId)
            .Select(aq => new { aq.GroupId })
            .FirstOrDefaultAsync();

        if (sourceQuestionnaire == null)
            return new List<ActiveQuestionnaireBase>();

        var completedQuestionnaires = await _context.Set<ActiveQuestionnaireModel>()
            .Where(aq => aq.GroupId == sourceQuestionnaire.GroupId
                      && aq.StudentCompletedAt != null
                      && aq.TeacherCompletedAt != null)
            .Include(aq => aq.Student)
            .Include(aq => aq.Teacher)
            .ToListAsync();

        return completedQuestionnaires.Select(aq => aq.ToBaseDto()).ToList();
    }
}
