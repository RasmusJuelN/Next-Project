
namespace API.Interfaces
{
    public interface IQuestionnaireTemplateService
    {
        Task<TemplateKeysetPaginationResult> GetTemplateBasesWithKeysetPagination(TemplateKeysetPaginationRequest request);
        Task<QuestionnaireTemplate> AddTemplate(QuestionnaireTemplateAdd request);
        Task<QuestionnaireTemplate> GetTemplateById(Guid id);
        Task<QuestionnaireTemplate> UpdateTemplate(Guid id, QuestionnaireTemplateUpdate updateRequest);
        Task<QuestionnaireTemplate> PatchTemplate(Guid id, QuestionnaireTemplatePatch patchRequest);
        Task DeleteTemplate(Guid id);
        Task<QuestionnaireTemplate> FinalizeTemplate(Guid id);

    }
}
