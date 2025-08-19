using Database.DTO.ActiveQuestionnaire;
using Database.Enums;
using Database.Extensions;
using Database.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Repository;

public class ActiveQuestionnaireRepository(Context context, ILoggerFactory loggerFactory) : IActiveQuestionnaireRepository
{
    private readonly Context _context = context;
    private readonly GenericRepository<ActiveQuestionnaireModel> _genericRepository = new(context, loggerFactory);

    public async Task<ActiveQuestionnaireBase> GetActiveQuestionnaireBaseAsync(Guid id)
    {
        ActiveQuestionnaireModel activeQuestionnaire = await _genericRepository.GetSingleAsync(a => a.Id == id,
            query => query.Include(a => a.Student).Include(a => a.Teacher)) ?? throw new Exception("Active questionnaire not found.");
        
        return activeQuestionnaire.ToBaseDto();
    }

    public async Task<ActiveQuestionnaire> GetFullActiveQuestionnaireAsync(Guid id)
    {
        ActiveQuestionnaireModel activeQuestionnaire = await _genericRepository.GetSingleAsync(a => a.Id == id,
            query => query.Include(a => a.Student).Include(a => a.Teacher).Include(a => a.QuestionnaireTemplate.Questions).ThenInclude(q => q.Options)) ?? throw new Exception("Active questionnaire not found.");
        
        return activeQuestionnaire.ToDto();
    }

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

    public async Task<bool> IsActiveQuestionnaireComplete(Guid activeQuestionnaireId)
    {
        ActiveQuestionnaireModel activeQuestionnaire = await _context.ActiveQuestionnaires.SingleAsync(a => a.Id == activeQuestionnaireId);

        return activeQuestionnaire.StudentCompletedAt.HasValue && activeQuestionnaire.TeacherCompletedAt.HasValue;
    }

    public async Task<bool> IsActiveQuestionnaireComplete(Guid activeQuestionnaireId, Guid userId)
    {
        ActiveQuestionnaireModel activeQuestionnaire = await _context.ActiveQuestionnaires.SingleAsync(a => a.Id == activeQuestionnaireId && (a.Student.Guid == userId || a.Teacher.Guid == userId));

        return activeQuestionnaire.StudentCompletedAt.HasValue && activeQuestionnaire.TeacherCompletedAt.HasValue;
    }

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
        
        return activeQuestionnaire.ToFullResponse();
    }

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
}
