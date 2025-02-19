using Database.DTO.QuestionnaireTemplate;
using Database.Models;

namespace Database.Repository;

public interface IQuestionnaireTemplateRepository : IGenericRepository<QuestionnaireTemplateModel>
{
    Task<QuestionnaireTemplateModel> UpdateAsync(QuestionnaireTemplateModel existingTemplate, QuestionnaireTemplateModel updatedTemplate);
    Task<QuestionnaireTemplateModel> PatchAsync(QuestionnaireTemplateModel existingTemplate, QuestionnaireTemplatePatch patchedTemplate);
}
