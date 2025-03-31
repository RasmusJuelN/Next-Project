using System.IdentityModel.Tokens.Jwt;
using API.DTO.Requests.ActiveQuestionnaire;
using API.DTO.Responses.ActiveQuestionnaire;
using API.Exceptions;
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
        [Authorize(AuthenticationSchemes = "AccessToken")]
        public async Task<ActionResult<ActiveQuestionnaireKeysetPaginationResultAdmin>> GetActiveQuestionnaires([FromQuery] ActiveQuestionnaireKeysetPaginationRequestFull request)
        {
            return Ok(await _questionnaireService.FetchActiveQuestionnaireBases(request));
        }

        [HttpPost("activate")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<ActiveQuestionnaire>> ActivateQuestionnaire([FromForm] ActivateQuestionnaire request)
        {
            return Ok(await _questionnaireService.ActivateTemplate(request));
        }

        [HttpGet("check")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "StudentAndTeacherOnly")]
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
        [Authorize(AuthenticationSchemes = "AccessToken")]
        public async Task<ActionResult<ActiveQuestionnaire>> GetActiveQuestionnaire(Guid id)
        {
            return Ok(await _questionnaireService.FetchActiveQuestionnaire(id));
        }
        
        [HttpPut("{id}/submitAnswer")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "StudentAndTeacherOnly")]
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

            try
            {
                await _questionnaireService.SubmitAnswers(id, userId, submission);
            }
            catch(HttpResponseException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }

            return Ok();
        }

        [HttpGet("{id}/getResponse")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "TeacherOnly")]
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
