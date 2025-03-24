using System.IdentityModel.Tokens.Jwt;
using API.DTO.Requests.ActiveQuestionnaire;
using API.DTO.Responses.ActiveQuestionnaire;
using API.Services;
using Database.DTO.ActiveQuestionnaire;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("activate")]
        public async Task<ActionResult<ActiveQuestionnaire>> ActivateQuestionnaire([FromForm] ActivateQuestionnaire request)
        {
            return Ok(await _questionnaireService.ActivateTemplate(request));
        }

        [Authorize(AuthenticationSchemes = "AccessToken")]
        [HttpGet("check")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
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

        [HttpGet("{id}")]
        public async Task<ActionResult<ActiveQuestionnaire>> GetActiveQuestionnaire(Guid id)
        {
            return Ok(await _questionnaireService.FetchActiveQuestionnaire(id));
        }
        
        [Authorize(AuthenticationSchemes = "AccessToken")]
        [HttpPut("{id}/submitAnswer")]
        public async Task<ActionResult> SubmitQuestionnaireAnswer(Guid id, [FromBody] AnswerSubmission submission)
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

            await _questionnaireService.SubmitAnswers(id, userId, submission);

            return Ok();
        }

        [Authorize(AuthenticationSchemes = "AccessToken")]
        [HttpGet("{id}/getResponse")]
        public async Task<ActionResult<List<FullResponse>>> GetActiveQuestionnaireResponses(Guid id)
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

            FullResponse response = await _questionnaireService.GetFullResponseAsync(id);

            if (userId != response.Student.User.Guid && userId != response.Teacher.User.Guid)
            {
                return Unauthorized();
            }
            else
            {
                return Ok(await _questionnaireService.GetFullResponseAsync(id));
            }
        }
    }
}
