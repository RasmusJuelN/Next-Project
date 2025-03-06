using System.IdentityModel.Tokens.Jwt;
using API.DTO.Requests.ActiveQuestionnaire;
using API.DTO.Responses.ActiveQuestionnaire;
using API.Services;
using Database.DTO.ActiveQuestionnaire;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActiveQuestionnaireController(ActiveQuestionnaireService questionnaireService) : ControllerBase
    {
        private readonly ActiveQuestionnaireService _questionnaireService = questionnaireService;

        [HttpGet]
        public async Task<ActionResult<ActiveQuestionnaireKeysetPaginationResult>> GetActiveQuestionnaires([FromQuery] ActiveQuestionnaireKeysetPaginationRequest request)
        {
            return Ok(await _questionnaireService.FetchActiveQuestionnaireBases(request));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActiveQuestionnaire>> GetActiveQuestionnaire(Guid id)
        {
            return Ok(await _questionnaireService.FetchActiveQuestionnaire(id));
        }

        [HttpPost("activate")]
        public async Task<ActionResult<ActiveQuestionnaire>> ActivateQuestionnaire([FromForm] ActivateQuestionnaire request)
        {
            return Ok(await _questionnaireService.ActivateTemplate(request));
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
