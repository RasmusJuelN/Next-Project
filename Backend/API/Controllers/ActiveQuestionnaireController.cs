using System.IdentityModel.Tokens.Jwt;
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
