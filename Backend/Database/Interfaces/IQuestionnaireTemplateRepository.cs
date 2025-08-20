using Database.DTO.QuestionnaireTemplate;
using Database.Enums;

namespace Database.Interfaces;

public interface IQuestionnaireTemplateRepository
{
    Task<QuestionnaireTemplate> AddAsync(QuestionnaireTemplateAdd questionnaire);
    Task<QuestionnaireTemplate> Update(Guid id, QuestionnaireTemplateUpdate updatedTemplate);
    Task<QuestionnaireTemplate> Patch(Guid id, QuestionnaireTemplatePatch patchedTemplate);
    Task DeleteAsync(Guid id);
    Task<QuestionnaireTemplateBase?> GetQuestionnaireTemplateBaseAsync(Guid id);
    Task<QuestionnaireTemplate?> GetFullQuestionnaireTemplateAsync(Guid id);
    Task<(List<QuestionnaireTemplateBase>, int)> PaginationQueryWithKeyset(
        int amount,
        Guid? cursorIdPosition,
        DateTime? cursorCreatedAtPosition,
        TemplateOrderingOptions sortOrder,
        string? titleQuery,
        Guid? idQuery,
        TemplateStatus? templateStatus);
        Task<QuestionnaireTemplate> FinalizeAsync(Guid id);

}
