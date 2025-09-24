using Database.DTO.QuestionnaireTemplate;
using Database.Enums;
using Database.Extensions;
using Database.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Repository;

public class QuestionnaireTemplateRepository(Context context, ILoggerFactory loggerFactory) : IQuestionnaireTemplateRepository
{
    private readonly Context _context = context;
    private readonly GenericRepository<QuestionnaireTemplateModel> _genericRepository = new(context, loggerFactory);

    public async Task<QuestionnaireTemplate> Update(Guid id, QuestionnaireTemplateUpdate updatedTemplate)
    {
        QuestionnaireTemplateModel existingTemplate = await _genericRepository.GetSingleAsync(t => t.Id == id,
            query => query.Include(t => t.Questions).ThenInclude(q => q.Options)) ?? throw new Exception("Template not found.");

        existingTemplate.Title = updatedTemplate.Title;
        existingTemplate.Description = updatedTemplate.Description;

        HashSet<int> updatedQuestionIds = [.. updatedTemplate.Questions.Where(q => q.Id is not null).Select(q => q.Id!.Value)];
        IEnumerable<QuestionnaireQuestionModel> oldQuestions = existingTemplate.Questions.Where(e => !updatedQuestionIds.Contains(e.Id));

        // Remove existing questions
        _context.RemoveRange(oldQuestions);

        foreach (QuestionnaireQuestionUpdate updatedQuestion in updatedTemplate.Questions)
        {
            QuestionnaireQuestionModel? existingQuestion = existingTemplate.Questions.SingleOrDefault(q => q.Id == updatedQuestion.Id);

            if (existingQuestion is not null)
            {
                // Update existing question and check its options
                existingQuestion.Prompt = updatedQuestion.Prompt;
                existingQuestion.AllowCustom = updatedQuestion.AllowCustom;

                HashSet<int> updatedOptionIds = [.. updatedQuestion.Options.Where(o => o.Id is not null).Select(o => o.Id!.Value)];
                IEnumerable<QuestionnaireOptionModel> oldOptions = existingQuestion.Options.Where(e => !updatedOptionIds.Contains(e.Id));

                // Remove existing options
                _context.RemoveRange(oldOptions);

                List<QuestionnaireOptionModel> newOptions = [];

                foreach (QuestionnaireOptionUpdate updatedOption in updatedQuestion.Options)
                {
                    QuestionnaireOptionModel? existingOption = existingQuestion.Options.SingleOrDefault(o => o.Id == updatedOption.Id);

                    if (existingOption is not null)
                    {
                        // Update existing option
                        existingOption.OptionValue = updatedOption.OptionValue;
                        existingOption.DisplayText = updatedOption.DisplayText;
                    }
                    else
                    {
                        // Create new option
                        QuestionnaireOptionModel newOption = updatedOption.ToModel();
                        existingQuestion.Options.Add(newOption);
                    }
                }
            }
            else
            {
                // Create new question
                QuestionnaireQuestionModel newQuestion = updatedQuestion.ToModel();
                existingTemplate.Questions.Add(newQuestion);
            }
        }

        return existingTemplate.ToDto();
    }

    public async Task<QuestionnaireTemplate> Patch(Guid id, QuestionnaireTemplatePatch patchedTemplate)
    {
        // TODO: Add/Port over existing custom SQL exceptions and use here
        QuestionnaireTemplateModel existingTemplate = await _genericRepository.GetSingleAsync(t => t.Id == id,
            query => query.Include(t => t.Questions).ThenInclude(q => q.Options)) ?? throw new Exception("Template not found.");

        existingTemplate.Title = patchedTemplate.TemplateTitle ?? existingTemplate.Title;
        existingTemplate.Description = patchedTemplate.Description ?? existingTemplate.Description;

        if (patchedTemplate.Questions is not null && patchedTemplate.Questions.Count != 0)
        {
            HashSet<int> patchedQuestionIds = [.. patchedTemplate.Questions.Select(q => q.Id)];
            IEnumerable<QuestionnaireQuestionModel> existingQuestions = existingTemplate.Questions.Where(e => patchedQuestionIds.Contains(e.Id));

            foreach (QuestionnaireQuestionModel existingQuestion in existingQuestions)
            {
                QuestionnaireQuestionPatch patchedQuestion = patchedTemplate.Questions.Single(q => q.Id == existingQuestion.Id);

                existingQuestion.Prompt = patchedQuestion.Prompt ?? existingQuestion.Prompt;
                existingQuestion.AllowCustom = patchedQuestion.AllowCustom ?? existingQuestion.AllowCustom;

                if (patchedQuestion.Options is not null && patchedQuestion.Options.Count != 0)
                {
                    HashSet<int> patchedOptionIds = [.. patchedQuestion.Options.Select(o => o.Id)];
                    IEnumerable<QuestionnaireOptionModel> existingOptions = existingQuestion.Options.Where(e => patchedOptionIds.Contains(e.Id));

                    foreach (QuestionnaireOptionModel existingOption in existingOptions)
                    {
                        QuestionnaireOptionPatch patchedOption = patchedQuestion.Options.Single(o => o.Id == existingOption.Id);

                        existingOption.OptionValue = patchedOption.OptionValue ?? existingOption.OptionValue;
                        existingOption.DisplayText = patchedOption.DisplayText ?? existingOption.DisplayText;
                    }
                }
            }
        }

        existingTemplate.LastUpated = DateTime.UtcNow;

        return existingTemplate.ToDto();
    }

    public async Task<QuestionnaireTemplateBase?> GetQuestionnaireTemplateBaseAsync(Guid id)
    {
        QuestionnaireTemplateModel? questionnaire = await _genericRepository.GetSingleAsync(t => t.Id == id);
        return questionnaire?.ToBaseDto();
    }

    public async Task<QuestionnaireTemplate?> GetFullQuestionnaireTemplateAsync(Guid id)
    {
        QuestionnaireTemplateModel? questionnaire = await _genericRepository.GetSingleAsync(t => t.Id == id,
            query => query.Include(t => t.Questions).ThenInclude(q => q.Options));
        return questionnaire?.ToDto();
    }

    public async Task<QuestionnaireTemplate> AddAsync(QuestionnaireTemplateAdd questionnaire)
    {
        QuestionnaireTemplateModel questionnaireTemplate = questionnaire.ToModel();
        await _genericRepository.AddAsync(questionnaireTemplate);
        return questionnaireTemplate.ToDto();
    }

    public async Task DeleteAsync(Guid id)
    {
        QuestionnaireTemplateModel existingTemplate = await _context.QuestionnaireTemplates.Include(q => q.ActiveQuestionnaires).SingleAsync(q => q.Id == id);

        if (existingTemplate.ActiveQuestionnaires.Count != 0)
        {
            _context.ActiveQuestionnaires.RemoveRange(existingTemplate.ActiveQuestionnaires);
        }

        _genericRepository.Delete(existingTemplate);
        await _context.SaveChangesAsync();
    }

    public async Task<(List<QuestionnaireTemplateBase>, int)> PaginationQueryWithKeyset(
        int amount,
        Guid? cursorIdPosition,
        DateTime? cursorCreatedAtPosition,
        TemplateOrderingOptions sortOrder,
        string? titleQuery,
        Guid? idQuery,
        TemplateStatus? templateStatus)
    {
        IQueryable<QuestionnaireTemplateModel> query = _genericRepository.GetAsQueryable();

        query = sortOrder.ApplyQueryOrdering(query);

        if (!string.IsNullOrEmpty(titleQuery))
        {
            query = query.Where(q => q.Title.Contains(titleQuery));
        }

        if (idQuery is not null)
        {
            query = query.Where(q => q.Id.ToString().Contains(idQuery.ToString()!));
        }

        if (templateStatus.HasValue)
        {
            var status = templateStatus.Value;
            query = query.Where(q => q.TemplateStatus == status);
        }

        int totalCount = await query.CountAsync();

        if (cursorIdPosition is not null && cursorCreatedAtPosition is not null)
        {
            if (sortOrder == TemplateOrderingOptions.CreatedAtAsc)
            {
                query = query.Where(q => q.CreatedAt > cursorCreatedAtPosition
                || q.CreatedAt == cursorCreatedAtPosition && q.Id > cursorIdPosition);
            }
            else
            {
                query = query.Where(q => q.CreatedAt < cursorCreatedAtPosition
                || q.CreatedAt == cursorCreatedAtPosition && q.Id < cursorIdPosition);
            }
        }

        List<QuestionnaireTemplateModel> questionnaireTemplates = await query.Take(amount).ToListAsync();
        List<QuestionnaireTemplateBase> questionnaireTemplateBases = [.. questionnaireTemplates.Select(t => t.ToBaseDto())];

        return (questionnaireTemplateBases, totalCount);
    }
    public async Task<QuestionnaireTemplate> FinalizeAsync(Guid id)
    {
        var existing = await _context.QuestionnaireTemplates
            .SingleOrDefaultAsync(t => t.Id == id)
            ?? throw new Exception("Template not found.");

        if (existing.TemplateStatus != TemplateStatus.Finalized)
        {
            existing.TemplateStatus = TemplateStatus.Finalized;
            existing.LastUpated = DateTime.UtcNow;
        }

        return existing.ToDto();
    }
}
