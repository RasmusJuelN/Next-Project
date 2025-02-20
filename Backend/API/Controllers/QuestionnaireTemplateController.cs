using API.Enums;
using API.Extensions;
using API.Models.Requests;
using API.Models.Responses;
using Database.DTO.QuestionnaireTemplate;
using Database.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionnaireTemplateController : ControllerBase
    {
        private readonly IQuestionnaireTemplateRepository _QuestionnaireTemplateRepository;
    
        public QuestionnaireTemplateController(IQuestionnaireTemplateRepository questionnaireTemplateRepository)
        {
            _QuestionnaireTemplateRepository = questionnaireTemplateRepository;
        }

        /// <summary>
        /// Retrieves a list of questionnaire templates based on the specified request parameters.
        /// </summary>
        /// <param name="request">The request parameters for retrieving questionnaire templates.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a list of <see cref="QuestionnaireTemplateBaseDto.PaginationResult"/>.
        /// </returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Get /api/questionnaire-template?PageSize=5&amp;Order=CreatedAtDesc
        ///     
        /// <para></para>
        /// queryCursor is a unique combination of the lastUpdated and Id values of the last item in the result.
        /// For subsequent queries, this can be included to indicate and control where the query should resume from.
        /// </remarks>
        /// <response code="200">Returns the list of questionnaire templates.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(QuestionnaireTemplateBaseDto.PaginationResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<QuestionnaireTemplateBaseDto.PaginationResult>> GetQuestionnaireTemplates([FromQuery] QuestionnaireTemplateApiRequests.PaginationQuery request)
        {
            IQueryable<QuestionnaireTemplateModel> query = _QuestionnaireTemplateRepository.GetAsQueryable();

            query = request.Order.ApplyQueryOrdering(query);

            if (!string.IsNullOrEmpty(request.Title))
            {
                query = query.Where(q => q.TemplateTitle.Contains(request.Title));
            }
            
            if (request.Id is not null)
            {
                query = query.Where(q => q.Id == request.Id);
            }

            if (!string.IsNullOrEmpty(request.QueryCursor))
            {
                DateTime cursorCreatedAt = DateTime.Parse(request.QueryCursor.Split('_')[0]);
                Guid cursorId = Guid.Parse(request.QueryCursor.Split('_')[1]);
                
                if (request.Order == QuestionnaireBaseTemplateOrdering.CreatedAtAsc)
                {
                    // We reverse the order so that we "walk" through the rows
                    // I.e. if we didn't reverse it, and was in the middle multiple rows
                    // it'd select from the "outside" of the result, instead of from the inside
                    // I.e. we go from:
                    // row3 < row4 < row5 cursor_position < row2 < row1
                    // To:
                    // row5 < row4 < row3 cursor_position < row2 < row1
                    query = query.Where(q => q.CreatedAt < cursorCreatedAt
                    || q.CreatedAt == cursorCreatedAt && q.Id < cursorId).Reverse();
                }
                else
                {
                    query = query.Where(q => q.CreatedAt > cursorCreatedAt
                    || q.CreatedAt == cursorCreatedAt && q.Id > cursorId).Reverse();
                }
            }

            int totalQueryCount = await query.CountAsync();
            query = query.Take(request.PageSize);

            List<QuestionnaireTemplateModel> questionnaireTemplates = await query.ToListAsync();

            List<QuestionnaireTemplateBaseDto.TemplateBase> questionnaireTemplatesDto = [.. questionnaireTemplates.Select(q => q.ToBaseDto())];
            QuestionnaireTemplateBaseDto.TemplateBase? lastTemplate = questionnaireTemplatesDto.Count != 0 ? questionnaireTemplatesDto.Last() : null;

            string? queryCursor = null;
            if (lastTemplate is not null)
            {
                queryCursor = $"{lastTemplate.CreatedAt:O}_{lastTemplate.Id}";
            }

            return Ok(new QuestionnaireTemplateBaseDto.PaginationResult()
            {
                TemplateBases = questionnaireTemplatesDto,
                QueryCursor = queryCursor,
                TotalCount = totalQueryCount
            });
        }

        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(QuestionnaireTemplateBaseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(QuestionnaireTemplateDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<QuestionnaireTemplateDto>> AddQuestionnaireTemplate([FromBody] QuestionnaireTemplateApiRequests.AddTemplate questionnaireTemplate)
        {
            QuestionnaireTemplateModel? existingTemplate = await _QuestionnaireTemplateRepository.GetSingleAsync(q => q.TemplateTitle == questionnaireTemplate.TemplateTitle);
            
            if (existingTemplate != null)
            {
                return Conflict(existingTemplate.ToDto());
            }
            
            QuestionnaireTemplateModel template = await _QuestionnaireTemplateRepository.AddAsync(questionnaireTemplate.ToModel());

            return CreatedAtRoute("", template.Id, template.ToDto());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(QuestionnaireTemplateDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<QuestionnaireTemplateDto>> GetQuestionnaireTemplate(Guid id)
        {
            QuestionnaireTemplateModel? template = await _QuestionnaireTemplateRepository.GetSingleAsync(q => q.Id == id,
                query => query.Include(q => q.Questions).ThenInclude(q => q.Options));

            if (template == null)
            {
                return NotFound();
            }

            return Ok(template.ToDto());
        }

        [HttpPut("{id}/update")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(QuestionnaireTemplateDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<QuestionnaireTemplateDto>> UpdateQuestionnaireTemplate(Guid id, [FromBody] QuestionnaireTemplateUpdateRequest updateRequest)
        {
            IQueryable<QuestionnaireTemplateModel> query = _QuestionnaireTemplateRepository.GetAsQueryable();
            QuestionnaireTemplateModel? existingTemplate = await query
                .AsNoTracking()
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id);
            
            if (existingTemplate is null)
            {
                return NotFound();
            }

            QuestionnaireTemplateModel updatedTemplate = await _QuestionnaireTemplateRepository.UpdateAsync(existingTemplate, updateRequest.ToModel(existingTemplate));

            return Ok(updatedTemplate.ToDto());
        }

        [HttpPatch("{id}/patch")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(QuestionnaireTemplateDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<QuestionnaireTemplateDto>> PatchQuestionnaireTemplate(Guid id, [FromBody] QuestionnaireTemplatePatch updateRequest)
        {
            QuestionnaireTemplateModel? existingModel = await _QuestionnaireTemplateRepository.GetSingleAsync(q => q.Id == id, query => query.Include(q => q.Questions).ThenInclude(o => o.Options));

            if (existingModel == null)
            {
                return NotFound();
            }

            QuestionnaireTemplateModel updatedModel = await _QuestionnaireTemplateRepository.PatchAsync(existingModel, updateRequest);

            return updatedModel.ToDto();
        }

        [HttpDelete("{id}/delete")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(QuestionnaireTemplateDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<QuestionnaireTemplateDto>> DeleteQuestionnaireTemplate(Guid id)
        {
            QuestionnaireTemplateModel? template = await _QuestionnaireTemplateRepository.GetSingleAsync(q => q.Id == id,
                query => query.Include(q => q.Questions).ThenInclude(q => q.Options));

            if (template == null)
            {
                return NotFound();
            }

            await _QuestionnaireTemplateRepository.DeleteAsync(template);
            return Ok(template.ToDto());
        }
    }
}
