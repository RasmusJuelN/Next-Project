using API.Extensions;
using API.Models.Requests;
using API.Models.Responses;
using Database.Models;
using Database.Repository;
using Microsoft.AspNetCore.Mvc;

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
            List<QuestionnaireTemplateModel> questionnaireTemplates = await _QuestionnaireRepository.GetAllAsync(
                q => q.OrderBy(c => c.CreatedAt).Skip(start).Take(amount));
            
            return [.. questionnaireTemplates.Select(q => q.ToDto())];
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddQuestionnaireTemplate([FromBody] QuestionnaireTemplateAddRequest questionnaireTemplate)
        {
            await _QuestionnaireRepository.AddAsync(questionnaireTemplate.ToModel());

            return Ok();
        }
    }
}
