
namespace API.Services;

/// <summary>
/// Provides business logic and data management services for questionnaire template operations.
/// This service handles the complete lifecycle of questionnaire templates including creation,
/// modification, retrieval, and deletion while ensuring data integrity and proper validation.
/// </summary>
/// <remarks>
/// The service implements comprehensive template management functionality:
/// <list type="bullet">
/// <item><description>Template creation and validation with question and option hierarchies</description></item>
/// <item><description>Paginated retrieval with advanced filtering and sorting capabilities</description></item>
/// <item><description>Full update and partial patch operations for template modifications</description></item>
/// <item><description>Template deletions</description></item>
/// </list>
/// All operations are performed through the Unit of Work pattern to ensure transactional consistency.
/// </remarks>

public class QuestionnaireTemplateService : IQuestionnaireTemplateService
{
    private readonly IUnitOfWork _unitOfWork;

    public QuestionnaireTemplateService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    /// <summary>
    /// Retrieves a list of questionnaire templates based on the specified request parameters.
    /// Only their base is included, I.e. collections/navigations are not included.
    /// </summary>
    /// <param name="request">The request parameters for retrieving questionnaire templates.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the paginated template results.
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
            request.Id,
            request.templateStatus
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

    /// <summary>
    /// Retrieves a complete questionnaire template by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the questionnaire template to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// the complete questionnaire template including all questions and options.
    /// </returns>
    /// <remarks>
    /// This method returns the full template hierarchy including:
    /// <list type="bullet">
    /// <item><description>Template metadata (title, description, creation info)</description></item>
    /// <item><description>All associated questions with their properties</description></item>
    /// <item><description>All answer options for each question</description></item>
    /// </list>
    /// The returned data is suitable for template editing and viewing.
    /// </remarks>
    /// <exception cref="SQLException.ItemNotFound">Thrown when no template exists with the specified ID.</exception>
    /// <exception cref="ArgumentException">Thrown when the provided ID is invalid.</exception>
    public async Task<QuestionnaireTemplate> GetTemplateById(Guid id)
    {
        QuestionnaireTemplate template = await _unitOfWork.QuestionnaireTemplate.GetFullQuestionnaireTemplateAsync(id) ?? throw new SQLException.ItemNotFound();
        return template;
    }

    /// <summary>
    /// Updates an existing questionnaire template with new data, replacing all modifiable fields.
    /// </summary>
    /// <param name="id">The unique identifier of the template to update.</param>
    /// <param name="updateRequest">The complete update data for the template.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// the updated questionnaire template with all changes applied.
    /// </returns>
    /// <remarks>
    /// This method performs a complete update operation, replacing all template data
    /// with the provided information. It handles the complex relationships between
    /// templates, questions, and options while maintaining referential integrity.
    /// Changes are applied atomically within a database transaction.
    /// </remarks>
    /// <exception cref="SQLException.ItemNotFound">Thrown when no template exists with the specified ID.</exception>
    /// <exception cref="ArgumentException">Thrown when the update request contains invalid data.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the template cannot be updated due to business rules.</exception>
    public async Task<QuestionnaireTemplate> UpdateTemplate(Guid id, QuestionnaireTemplateUpdate updateRequest)
    {
        QuestionnaireTemplate updatedTemplate = await _unitOfWork.QuestionnaireTemplate.Update(id, updateRequest);
        await _unitOfWork.SaveChangesAsync();

        return updatedTemplate;
    }

    /// <summary>
    /// Applies partial updates to an existing questionnaire template using patch semantics.
    /// </summary>
    /// <param name="id">The unique identifier of the template to patch.</param>
    /// <param name="patchRequest">The partial update data containing only fields to be modified.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// the questionnaire template with the specified changes applied.
    /// </returns>
    /// <remarks>
    /// This method allows for selective updates of template fields without requiring
    /// a complete replacement of the template data. Only the fields specified in the
    /// patch request are modified, leaving other fields unchanged. This is particularly
    /// useful for minor modifications and incremental updates.
    /// </remarks>
    /// <exception cref="SQLException.ItemNotFound">Thrown when no template exists with the specified ID.</exception>
    /// <exception cref="ArgumentException">Thrown when the patch request contains invalid data.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the template cannot be patched due to business rules.</exception>
    public async Task<QuestionnaireTemplate> PatchTemplate(Guid id, QuestionnaireTemplatePatch patchRequest)
    {
        QuestionnaireTemplate patchedTemplate = await _unitOfWork.QuestionnaireTemplate.Patch(id, patchRequest);
        await _unitOfWork.SaveChangesAsync();

        return patchedTemplate;
    }

    /// <summary>
    /// Permanently removes a questionnaire template from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the template to delete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This operation is irreversible and will permanently remove the template
    /// from the database. Consider the impact on associated active questionnaires
    /// and ensure proper authorization before calling this method. The deletion
    /// process includes proper cleanup of all related data and dependencies.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the template cannot be deleted due to existing dependencies
    /// or active questionnaires using this template.
    /// </exception>
    /// <exception cref="SQLException">Thrown when a database error occurs during deletion.</exception>
    public async Task DeleteTemplate(Guid id)
    {
        await _unitOfWork.QuestionnaireTemplate.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

public async Task<QuestionnaireTemplate> FinalizeTemplate(Guid id)
{
    var finalized = await _unitOfWork.QuestionnaireTemplate.FinalizeAsync(id);
    await _unitOfWork.SaveChangesAsync();
    return finalized;
}
}
