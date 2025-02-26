using API.DTO.Requests.ActiveQuestionnaire;
using API.Services;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActiveQuestionnaireController(ActiveQuestionnaireService questionnaireService) : ControllerBase
    {
        private readonly ActiveQuestionnaireService _questionnaireService = questionnaireService;

        [HttpPost("activate")]
        public async Task<ActionResult> AddQuestionnaire([FromForm] ActivateQuestionnaire request)
        {
            ActiveQuestionnaireModel activeQuestionnaire = await _questionnaireService.ActivateTemplate(request);

            return Ok();
        }

        [HttpGet("checkActive")]
        public async Task<ActionResult<Guid?>> GetOldestActiveQuestionnaireForUser()
        {
            string token;

            try
            {
                token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer", "");
            }
            catch (Exception)
            {
                return Unauthorized();   
            }

            throw new NotSupportedException();
        }
    }
}
