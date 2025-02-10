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

        [HttpPost("add")]
        public async Task<IActionResult> AddQuestionnaireTemplate([FromBody] QuestionnaireTemplateAddRequest questionnaireTemplate)
        {
            await _QuestionnaireRepository.AddAsync(questionnaireTemplate.ToModel());

            return Ok();
        }

        [HttpGet("amount")]
        public ActionResult<int> GetAmountOfQuestionnaireTemplates()
        {
            return Ok(_QuestionnaireRepository.GetCount());
        }
    }
}
