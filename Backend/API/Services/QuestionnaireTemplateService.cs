using API.DTO.Requests.QuestionnaireTemplate;
using API.Exceptions;
using API.Interfaces;
using Database.DTO.QuestionnaireTemplate;
using Database.Models;
using API.DTO.Responses.QuestionnaireTemplate;

namespace API.Services;

public class QuestionnaireTemplateService(IUnitOfWork unitOfWork)
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;


    /// <summary>
    /// Retrieves a list of questionnaire templates based on the specified request parameters.
    /// Only their base is included, I.e. collections/navigations are not included.
    /// </summary>
    /// <param name="request">The request parameters for retrieving questionnaire templates.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="TemplateKeysetPaginationResult"/> object.
    /// </returns>
    public async Task<TemplateKeysetPaginationResult> GetTemplateBasesWithKeysetPagination(TemplateKeysetPaginationRequest request)
    {
        DateTime? cursorCreatedAt = null;
        Guid? cursorId = null;

        if (!string.IsNullOrEmpty(request.QueryCursor))
        {
            cursorCreatedAt = DateTime.Parse(request.QueryCursor.Split('_')[0]);
            cursorId = Guid.Parse(request.QueryCursor.Split('_')[1]);
        }

        (List<QuestionnaireTemplateBase> questionnaireTemplateBases, int totalCount) = await _unitOfWork.QuestionnaireTemplate
        .PaginationQueryWithKeyset(
            request.PageSize,
            cursorId,
            cursorCreatedAt,
            request.Order,
            request.Title,
            request.Id
        );

        QuestionnaireTemplateBase? lastTemplate = questionnaireTemplateBases.Count != 0 ? questionnaireTemplateBases.Last() : null;

        string? queryCursor = null;
        if (lastTemplate is not null)
        {
            queryCursor = $"{lastTemplate.CreatedAt:O}_{lastTemplate.Id}";
        }

        return new TemplateKeysetPaginationResult()
        {
            TemplateBases = questionnaireTemplateBases,
            QueryCursor = queryCursor,
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Adds a new questionnaire template to the database.
    /// </summary>
    /// <param name="request">The template to be added, represented by <see cref="QuestionnaireTemplateAdd"/>.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the added <see cref="QuestionnaireTemplateModel"/>.
    /// </returns>
    public async Task<QuestionnaireTemplate> AddTemplate(QuestionnaireTemplateAdd request)
    {
        QuestionnaireTemplate template = await _unitOfWork.QuestionnaireTemplate.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();
        return template;
    }

    public async Task<QuestionnaireTemplate> GetTemplateById(Guid id)
    {
        QuestionnaireTemplate template = await _unitOfWork.QuestionnaireTemplate.GetFullQuestionnaireTemplateAsync(id) ?? throw new SQLException.ItemNotFound();
        return template;
    }

    public async Task<QuestionnaireTemplate> UpdateTemplate(Guid id, QuestionnaireTemplateUpdate updateRequest)
    {
        QuestionnaireTemplate updatedTemplate = await _unitOfWork.QuestionnaireTemplate.Update(id, updateRequest);
        await _unitOfWork.SaveChangesAsync();

        return updatedTemplate;
    }

    public async Task<QuestionnaireTemplate> PatchTemplate(Guid id, QuestionnaireTemplatePatch patchRequest)
    {
        QuestionnaireTemplate patchedTemplate = await _unitOfWork.QuestionnaireTemplate.Patch(id, patchRequest);
        await _unitOfWork.SaveChangesAsync();

        return patchedTemplate;
    }

    public async Task DeleteTemplate(Guid id)
    {
        await _unitOfWork.QuestionnaireTemplate.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();

        return;
    }
}
