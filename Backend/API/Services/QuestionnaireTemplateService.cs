using API.Enums;
using API.Exceptions;
using API.Extensions;
using API.Interfaces;
using API.Models.Requests;
using API.Models.Responses;
using Database.DTO.QuestionnaireTemplate;
using Database.Models;
using Microsoft.EntityFrameworkCore;

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
    /// A task that represents the asynchronous operation. The task result contains a <see cref="QuestionnaireTemplateBaseDto.PaginationResult"/> object.
    /// </returns>
    public async Task<QuestionnaireTemplateBaseDto.PaginationResult> GetTemplateBasesWithKeysetPagination(QuestionnaireTemplateApiRequests.PaginationQuery request)
    {
        IQueryable<QuestionnaireTemplateModel> query = _unitOfWork.QuestionnaireTemplate.GetAsQueryable();

        query = request.Order.ApplyQueryOrdering(query);

        if (!string.IsNullOrEmpty(request.Title))
        {
            query = query.Where(q => q.TemplateTitle.Contains(request.Title));
        }
        
        if (request.Id is not null)
        {
            query = query.Where(q => q.Id == request.Id);
        }

        int totalQueryCount = await query.CountAsync();

        if (!string.IsNullOrEmpty(request.QueryCursor))
        {
            DateTime cursorCreatedAt = DateTime.Parse(request.QueryCursor.Split('_')[0]);
            Guid cursorId = Guid.Parse(request.QueryCursor.Split('_')[1]);
            
            if (request.Order == QuestionnaireBaseTemplateOrdering.CreatedAtAsc)
            {
                query = query.Where(q => q.CreatedAt > cursorCreatedAt
                || q.CreatedAt == cursorCreatedAt && q.Id > cursorId);
            }
            else
            {
                query = query.Where(q => q.CreatedAt < cursorCreatedAt
                || q.CreatedAt == cursorCreatedAt && q.Id < cursorId);
            }
        }

        query = query.Take(request.PageSize);

        List<QuestionnaireTemplateModel> questionnaireTemplates = await query.ToListAsync();
        List<QuestionnaireTemplateBaseDto.TemplateBase> questionnaireTemplatesDto = [.. questionnaireTemplates.Select(q => q.ToBaseDto())];
        QuestionnaireTemplateBaseDto.TemplateBase? lastTemplate = questionnaireTemplatesDto.Count != 0 ? questionnaireTemplatesDto.Last() : null;

        string? queryCursor = null;
        if (lastTemplate is not null)
        {
            queryCursor = $"{lastTemplate.CreatedAt:O}_{lastTemplate.Id}";
        }

        return new QuestionnaireTemplateBaseDto.PaginationResult()
        {
            TemplateBases = questionnaireTemplatesDto,
            QueryCursor = queryCursor,
            TotalCount = totalQueryCount
        };
    }

    /// <summary>
    /// Adds a new questionnaire template to the database.
    /// </summary>
    /// <param name="template">The template to be added, represented by <see cref="QuestionnaireTemplateApiRequests.AddTemplate"/>.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the added <see cref="QuestionnaireTemplateModel"/>.
    /// </returns>
    public async Task<QuestionnaireTemplateDto> AddTemplate(QuestionnaireTemplateApiRequests.AddTemplate template)
    {
        if (await TemplateExists(template.TemplateTitle)) throw new SQLException.ItemAlreadyExists();
        
        QuestionnaireTemplateModel addedTemplate = template.ToModel();

        await _unitOfWork.QuestionnaireTemplate.AddAsync(addedTemplate);
        await _unitOfWork.SaveChangesAsync();

        return addedTemplate.ToDto();
    }

    public async Task<QuestionnaireTemplateDto> GetTemplateById(Guid id)
    {
        QuestionnaireTemplateModel template = await _unitOfWork.QuestionnaireTemplate.GetSingleAsync(t =>
            t.Id == id, query => query.Include(q => q.Questions).ThenInclude(o => o.Options)) ?? throw new SQLException.ItemNotFound();

        return template.ToDto();
    }

    public async Task<QuestionnaireTemplateDto> UpdateTemplate(Guid id, QuestionnaireTemplateUpdateRequest updateRequest)
    {
        QuestionnaireTemplateModel existingTemplate = await _unitOfWork.QuestionnaireTemplate.GetSingleAsync(t =>
            t.Id == id, query => query.AsNoTracking().Include(q => q.Questions).ThenInclude(o => o.Options)) ?? throw new SQLException.ItemNotFound();

        existingTemplate = _unitOfWork.QuestionnaireTemplate.Update(existingTemplate, updateRequest.ToModel(existingTemplate));
        await _unitOfWork.SaveChangesAsync();

        return existingTemplate.ToDto();
    }

    public async Task<QuestionnaireTemplateDto> PatchTemplate(Guid id, QuestionnaireTemplatePatch patchRequest)
    {
        QuestionnaireTemplateModel existingTemplate = await _unitOfWork.QuestionnaireTemplate.GetSingleAsync(t =>
            t.Id == id, query => query.Include(q => q.Questions).ThenInclude(o => o.Options)) ?? throw new SQLException.ItemNotFound();
        
        existingTemplate = _unitOfWork.QuestionnaireTemplate.Patch(existingTemplate, patchRequest);
        await _unitOfWork.SaveChangesAsync();

        return existingTemplate.ToDto();
    }

    public async Task DeleteTemplate(Guid id)
    {
        QuestionnaireTemplateModel existingTemplate = await _unitOfWork.QuestionnaireTemplate.GetSingleAsync(t => t.Id == id) ?? throw new SQLException.ItemNotFound();
        
        _unitOfWork.QuestionnaireTemplate.Delete(existingTemplate);
        await _unitOfWork.SaveChangesAsync();

        return;
    }

    /// <summary>
    /// Checks if a template with the specified title exists in the database.
    /// </summary>
    /// <param name="id">The id of the template to check for.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the template exists.
    /// </returns>
    public async Task<bool> TemplateExists(Guid id)
    {
        return await _unitOfWork.QuestionnaireTemplate.GetSingleAsync(t => t.Id == id) is not null;
    }

    /// <summary>
    /// Checks if a template with the specified title exists in the database.
    /// </summary>
    /// <param name="templateTitle">The title of the template to check for.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the template exists.
    /// </returns>
    public async Task<bool> TemplateExists(string templateTitle)
    {
        return await _unitOfWork.QuestionnaireTemplate.GetSingleAsync(t => t.TemplateTitle == templateTitle) is not null;
    }
}
