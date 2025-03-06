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
            query => query.Include(a => a.Student).Include(a => a.Teacher).Include(a => a.QuestionnaireTemplate.Questions)) ?? throw new Exception("Active questionnaire not found.");
        
        return activeQuestionnaire.ToDto();
    }

    public async Task<ActiveQuestionnaire> ActivateQuestionnaireAsync(
        Guid questionnaireTemplateId,
        Guid studentId,
        Guid teacherId)
    {
        StudentModel student = await _context.Users.OfType<StudentModel>().SingleAsync(u => u.Guid == studentId);
        TeacherModel teacher = await _context.Users.OfType<TeacherModel>().SingleAsync(u => u.Guid == teacherId);

        QuestionnaireTemplateModel questionnaireTemplate = await _context.QuestionnaireTemplates
            .Include(t => t.Questions)
            .ThenInclude(q => q.Options)
            .SingleAsync(t => t.Id == questionnaireTemplateId);

        ActiveQuestionnaireModel activeQuestionnaire = new()
        {
            Title = questionnaireTemplate.Title,
            Description = questionnaireTemplate.Description,
            Student = student,
            Teacher = teacher,
            QuestionnaireTemplate = questionnaireTemplate
        };

        await _genericRepository.AddAsync(activeQuestionnaire);

        return activeQuestionnaire.ToDto();
    }

    public async Task<(List<ActiveQuestionnaireBase>, int)> PaginationQueryWithKeyset(
        int amount,
        Guid? cursorIdPosition,
        DateTime? cursorActivatedAtPosition,
        ActiveQuestionnaireOrderingOptions sortOrder,
        string? titleQuery,
        Guid? idQuery)
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
        }
    }
}
