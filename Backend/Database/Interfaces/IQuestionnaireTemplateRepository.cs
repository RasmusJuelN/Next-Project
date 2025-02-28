using Database.DTO.QuestionnaireTemplate;
using Database.Models;

namespace Database.Interfaces;

public interface IQuestionnaireTemplateRepository : IGenericRepository<QuestionnaireTemplateModel>
{
    QuestionnaireTemplateModel Update(QuestionnaireTemplateModel existingTemplate, QuestionnaireTemplateModel updatedTemplate);
    QuestionnaireTemplateModel Patch(QuestionnaireTemplateModel existingTemplate, QuestionnaireTemplatePatch patchedTemplate);
    Task<QuestionnaireTemplateModel?> GetEntireTemplate(Guid id);
}
