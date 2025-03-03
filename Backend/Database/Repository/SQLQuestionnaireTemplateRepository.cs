using Database.DTO.QuestionnaireTemplate;
using Database.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Repository;

public class SQLQuestionnaireTemplateRepository(Context context) : SQLGenericRepository<QuestionnaireTemplateModel>(context, new LoggerFactory()), IQuestionnaireTemplateRepository
{
    private readonly Context _context = context;

    public QuestionnaireTemplateModel Update(QuestionnaireTemplateModel existingTemplate, QuestionnaireTemplateModel updatedTemplate)
    {
        // TODO: The logic of updating values should probably happen in here instead of from the DTOs
        HashSet<int> updatedQuestionIds = [.. updatedTemplate.Questions.Select(q => q.Id)];
        IEnumerable<QuestionnaireQuestionModel> oldQuestions = existingTemplate.Questions.Where(e => !updatedQuestionIds.Contains(e.Id));
        
        foreach (QuestionnaireQuestionModel oldQuestion in oldQuestions)
        {
            _context.Entry(oldQuestion).State = EntityState.Deleted;
        }

        foreach (QuestionnaireQuestionModel updatedQuestion in updatedTemplate.Questions)
        {
            QuestionnaireQuestionModel existingQuestion = existingTemplate.Questions.Single(q => q.Id == updatedQuestion.Id);

            HashSet<int> updatedOptionIds = [.. updatedQuestion.Options.Select(o => o.Id)];
            IEnumerable<QuestionnaireOptionModel> oldOptions = existingQuestion.Options.Where(e => !updatedOptionIds.Contains(e.Id));

            foreach (QuestionnaireOptionModel oldOption in oldOptions)
            {
                _context.Entry(oldOption).State = EntityState.Deleted;
            }
        }

        _context.Update(updatedTemplate);

        return updatedTemplate;
    }

    public QuestionnaireTemplateModel Patch(QuestionnaireTemplateModel existingTemplate, QuestionnaireTemplatePatch patchedTemplate)
    {
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

        return existingTemplate;
    }

    public async Task<QuestionnaireTemplateModel?> GetEntireTemplate(Guid id)
    {
        return await GetSingleAsync(t => t.Id == id, query => query.Include(t => t.Questions).ThenInclude(q => q.Options));
    }
}
