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
        public async Task<List<QuestionnaireTemplateResponse>> GetQuestionnaireTemplates([FromQuery] PaginationRequest request)
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
            
            return [.. questionnaireTemplates.Select(q => q.ToDto())];
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionnaireTemplateResponse>> GetQuestionnaireTemplate(Guid id)
        {
            QuestionnaireTemplateModel? template = await _QuestionnaireRepository.GetSingleAsync(q => q.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            return template.ToDto();
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddQuestionnaireTemplate([FromBody] QuestionnaireTemplateAddRequest questionnaireTemplate)
        {
            QuestionnaireTemplateModel template = await _QuestionnaireRepository.AddAsync(questionnaireTemplate.ToModel());

            return CreatedAtRoute("", template.Id, template.ToDto());
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<QuestionnaireTemplateResponse>> DeleteQuestionnaireTemplate(Guid id)
        {
            QuestionnaireTemplateModel? template = await _QuestionnaireRepository.GetSingleAsync(q => q.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            await _QuestionnaireRepository.DeleteAsync(template);
            return Ok(template.ToDto());
        }

        [HttpGet("amount")]
        public ActionResult<int> GetAmountOfQuestionnaireTemplates()
        {
            return Ok(_QuestionnaireRepository.GetCount());
        }
    }
}
