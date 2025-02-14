using System.Reflection;
using API.Attributes;
using API.Enums;
using API.Extensions;
using API.Models.Requests;
using API.Models.Responses;
using Database.Models;
using Database.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionnaireTemplateController(
        IGenericRepository<QuestionnaireTemplateModel> QuestionnaireRepository) : ControllerBase
    {
        private readonly IGenericRepository<QuestionnaireTemplateModel> _QuestionnaireRepository = QuestionnaireRepository;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(List<QuestionnaireTemplateBaseDto.PaginationResult>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<QuestionnaireTemplateBaseDto.PaginationResult>>> GetQuestionnaireTemplates([FromQuery] QuestionnaireBaseTemplateRequests.Get request)
        {
            IQueryable<QuestionnaireTemplateModel> query = _QuestionnaireRepository.GetAsQueryable();

            query = request.Order.ApplyQueryOrdering(query);

            if (!string.IsNullOrEmpty(request.Title))
            {
                query = query.Where(q => q.TemplateTitle.Contains(request.Title));
            }
            
            if (request.Id is not null)
            {
                query = query.Where(q => q.Id == request.Id);
            }

            if (request.NextCursor is not null)
            {
                if (request.Order == QuestionnaireBaseTemplateOrdering.CreatedAtAsc)
                {
                    // We reverse the order so that we "walk" through the rows
                    // I.e. if we didn't reverse it, and was in the middle multiple rows
                    // it'd select from the "outside" of the result, instead of from the inside
                    // I.e. we go from:
                    // row3 < row4 < row5 cursor_position < row2 < row1
                    // To:
                    // row5 < row4 < row3 cursor_position < row2 < row1
                    query = query.Where(q => q.CreatedAt < request.NextCursor.CreatedAt
                    || q.CreatedAt == request.NextCursor.CreatedAt && q.Id < request.NextCursor.Id).Reverse();
                }
                else
                {
                    query = query.Where(q => q.CreatedAt > request.NextCursor.CreatedAt
                    || q.CreatedAt == request.NextCursor.CreatedAt && q.Id > request.NextCursor.Id).Reverse();
                }
            }

            query = query.Take(request.PageSize);

            List<QuestionnaireTemplateModel> questionnaireTemplates = await query.ToListAsync();

            List<QuestionnaireTemplateBaseDto.TemplateBase> questionnaireTemplatesDto = [.. questionnaireTemplates.Select(q => q.ToBaseDto())];
            QuestionnaireTemplateBaseDto.TemplateBase? lastTemplate = questionnaireTemplatesDto.Count != 0 ? questionnaireTemplatesDto.Last() : null;

            QuestionnaireTemplateBaseDto.NextCursor nextCursor;
            if (lastTemplate is not null)
            {
                nextCursor = new()
                {
                    CreatedAt = lastTemplate.CreatedAt,
                    Id = lastTemplate.Id,
                };
            }
            else
            {
                nextCursor = new();
            }

            return Ok(new QuestionnaireTemplateBaseDto.PaginationResult()
            {
                TemplateBases = questionnaireTemplatesDto,
                NextCursor = nextCursor
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(QuestionnaireTemplateDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<QuestionnaireTemplateDto>> GetQuestionnaireTemplate(Guid id)
        {
            QuestionnaireTemplateModel? template = await _QuestionnaireRepository.GetSingleAsync(q => q.Id == id,
                query => query.Include(q => q.Questions).ThenInclude(q => q.Options));

            if (template == null)
            {
                return NotFound();
            }

            return Ok(template.ToDto());
        }

        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(QuestionnaireTemplateBaseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(QuestionnaireTemplateDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<QuestionnaireTemplateDto>> AddQuestionnaireTemplate([FromBody] QuestionnaireTemplateAddRequest questionnaireTemplate)
        {
            QuestionnaireTemplateModel? existingTemplate = await _QuestionnaireRepository.GetSingleAsync(q => q.TemplateTitle == questionnaireTemplate.TemplateTitle);
            
            if (existingTemplate != null)
            {
                return Conflict(existingTemplate.ToDto());
            }
            
            QuestionnaireTemplateModel template = await _QuestionnaireRepository.AddAsync(questionnaireTemplate.ToModel());

            return CreatedAtRoute("", template.Id, template.ToDto());
        }

        [HttpPost("update")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(QuestionnaireTemplateDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<QuestionnaireTemplateDto>> UpdateQuestionnaireTemplate([FromBody] QuestionnaireTemplateUpdateRequest updateRequest)
        {
            QuestionnaireTemplateModel? existingModel = await _QuestionnaireRepository.GetSingleAsync(q => q.Id == updateRequest.Id, query => query.Include(q => q.Questions).ThenInclude(o => o.Options));
            
            if (existingModel == null)
            {
                return NotFound();
            }

            QuestionnaireTemplateModel updatedTemplate = await _QuestionnaireRepository.UpdateAsync(updateRequest.ToModel(), existingModel);

            throw new NotImplementedException();
        }

        [HttpPatch("patch/{id}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(QuestionnaireTemplateDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<QuestionnaireTemplateDto>> PatchQuestionnaireTemplate(Guid id, [FromBody] QuestionnaireBaseTemplateRequests.Patch updateRequest)
        {
            QuestionnaireTemplateModel? existingModel = await _QuestionnaireRepository.GetSingleAsync(q => q.Id == id, query => query.Include(q => q.Questions).ThenInclude(o => o.Options));

            if (existingModel == null)
            {
                return NotFound();
            }

            QuestionnaireTemplateModel updatedModel = await _QuestionnaireRepository.PatchAsync(existingModel, updateRequest);

            return updatedModel.ToDto();
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(QuestionnaireTemplateDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<QuestionnaireTemplateDto>> DeleteQuestionnaireTemplate(Guid id)
        {
            QuestionnaireTemplateModel? template = await _QuestionnaireRepository.GetSingleAsync(q => q.Id == id,
                query => query.Include(q => q.Questions).ThenInclude(q => q.Options));

            if (template == null)
            {
                return NotFound();
            }

            await _QuestionnaireRepository.DeleteAsync(template);
            return Ok(template.ToDto());
        }

        [HttpGet("amount")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<int> GetAmountOfQuestionnaireTemplates()
        {
            return Ok(_QuestionnaireRepository.GetCount());
        }
    }
}
