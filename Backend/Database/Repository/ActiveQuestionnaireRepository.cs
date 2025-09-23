using Database.DTO.ActiveQuestionnaire;
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

    /// <summary>
    /// Creates a new active questionnaire instance from a template and assigns it to specific participants.
    /// </summary>
    /// <param name="questionnaireTemplateId">The ID of the questionnaire template to activate.</param>
    /// <param name="studentId">The GUID of the student participant.</param>
    /// <param name="teacherId">The GUID of the teacher participant.</param>
    /// <returns>The newly created ActiveQuestionnaire instance with complete structure.</returns>
    /// <exception cref="InvalidOperationException">Thrown when any of the specified entities (template, student, teacher) are not found.</exception>
    /// <remarks>
    /// This method efficiently retrieves participants from local context when available to minimize database queries.
    /// The created questionnaire inherits title and description from the template and establishes participant relationships.
    /// The template structure including questions and options is included for immediate use.
    /// </remarks>
    public async Task<ActiveQuestionnaire> ActivateQuestionnaireAsync(
        Guid questionnaireTemplateId,
        Guid studentId,
        Guid teacherId,
        Guid groupId)
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
            GroupId = groupId 
        };

        await _genericRepository.AddAsync(activeQuestionnaire);

        return activeQuestionnaire.ToDto();
    }

    /// <summary>
    /// Performs paginated retrieval of active questionnaires with comprehensive filtering and sorting options using keyset pagination.
    /// </summary>
    /// <param name="amount">The number of questionnaires to retrieve per page.</param>
    /// <param name="sortOrder">The ordering criteria for the results.</param>
    /// <param name="cursorIdPosition">Optional cursor ID for pagination continuation.</param>
    /// <param name="cursorActivatedAtPosition">Optional cursor activation timestamp for pagination continuation.</param>
    /// <param name="titleQuery">Optional filter by questionnaire title (partial match).</param>
    /// <param name="student">Optional filter by student name or username (partial match).</param>
    /// <param name="teacher">Optional filter by teacher name or username (partial match).</param>
    /// <param name="idQuery">Optional filter by questionnaire ID (partial match).</param>
    /// <param name="userId">Optional filter to show questionnaires assigned to a specific user (student or teacher).</param>
    /// <param name="onlyStudentCompleted">When true, shows only questionnaires where students have completed their responses.</param>
    /// <param name="onlyTeacherCompleted">When true, shows only questionnaires where teachers have completed their responses.</param>
    /// <returns>A tuple containing the list of questionnaire base DTOs and the total count matching the criteria.</returns>
    /// <remarks>
    /// Uses keyset pagination for consistent performance with large datasets. The cursor parameters work together
    /// to maintain stable pagination even when new records are added. Multiple filter options can be combined
    /// to create complex queries. Includes participant information for efficient display without additional queries.
    /// </remarks>
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
        bool onlyTeacherCompleted = false)
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
            .Include(a => a.StudentAnswers)
            .ThenInclude(a => a.Option)
            .Include(a => a.TeacherAnswers)
            .ThenInclude(a => a.Question)
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
        UserBaseModel user = await _context.Users.SingleAsync(u => u.Guid == userId);

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

        return [.. activeQuestionnaires.Select(a => a.ToFullStudentRespondsDate()) ];
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
            .Where(a => a.Student.Guid == studentid && a.QuestionnaireTemplate.Id == templateid && a.StudentCompletedAt.HasValue)
            .ToListAsync();

        return [.. activeQuestionnaires.Select(a => a.ToFullStudentRespondsDate())];
    }


}
