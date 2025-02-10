using System.Linq.Expressions;
using API.Enums;
using API.Extensions;
using API.Models.Requests;
using API.Models.Responses;
using Database.Models;
using Database.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<List<QuestionnaireTemplateBaseDto>>> GetQuestionnaireTemplates([FromQuery] PaginationRequest request)
        {
            int start = (request.Page - 1) * request.PageSize;
            int amount = request.PageSize;

            IQueryable<QuestionnaireTemplateModel> query = _QuestionnaireRepository.GetAsQueryable();

            if (request.SearchType != null && !string.IsNullOrEmpty(request.SearchTerm))
            {
                query = request.SearchType switch
                {
                    SearchTypes.Title => query.Where(q => q.TemplateTitle.Contains(request.SearchTerm)),
                    SearchTypes.Id => query.Where(q => q.Id.ToString().Contains(request.SearchTerm)),
                    _ => query
                };
            }

            query = query.OrderBy(c => c.CreatedAt).Skip(start).Take(amount);

            List<QuestionnaireTemplateModel> questionnaireTemplates = await query.ToListAsync();
            
            return Ok(questionnaireTemplates.Select(q => q.ToBaseDto()).ToList());
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

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
