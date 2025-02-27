using System.IdentityModel.Tokens.Jwt;
using API.DTO.Requests.ActiveQuestionnaire;
using API.DTO.Responses.ActiveQuestionnaire;
using API.Services;
using Database.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActiveQuestionnaireController(ActiveQuestionnaireService questionnaireService) : ControllerBase
    {
        private readonly ActiveQuestionnaireService _questionnaireService = questionnaireService;

        [HttpGet]
        public async Task<ActionResult<List<FetchActiveQuestionnaireBase>>> GetActiveQuestionnaires()
        {
            List<FetchActiveQuestionnaireBase> activeQuestionnaireBases = await _questionnaireService.FetchActiveQuestionnaireBases();
            return Ok(activeQuestionnaireBases);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FetchActiveQuestionnaire>> GetActiveQuestionnaire(Guid id)
        {
            return Ok(await _questionnaireService.FetchActiveQuestionnaire(id));
        }

        [HttpPost("activate")]
        public async Task<ActionResult<FetchActiveQuestionnaire>> AddQuestionnaire([FromForm] ActivateQuestionnaire request)
        {
            ActiveQuestionnaireModel activeQuestionnaire = await _questionnaireService.ActivateTemplate(request);

            return Ok(activeQuestionnaire);
        }

        [HttpGet("check")]
        public async Task<ActionResult<Guid?>> CheckIfUserHasActiveQuestionnaire()
        {
            Guid userId;
            try
            {
                userId = Guid.Parse(User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
            }
            catch (Exception)
            {
                return Unauthorized();   
            }
            
            return await _questionnaireService.GetOldestActiveQuestionnaireForUser(userId);
        }
    }
}
