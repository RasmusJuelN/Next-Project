using System.IdentityModel.Tokens.Jwt;
using API.DTO.Requests.ActiveQuestionnaire;
using API.DTO.Requests.User;
using API.DTO.Responses.ActiveQuestionnaire;
using API.DTO.Responses.User;
using API.Exceptions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserService userService) : ControllerBase
    {
        private readonly UserService _userService = userService;

        [HttpGet]
        [ProducesResponseType(typeof(UserQueryPaginationResult), StatusCodes.Status200OK)]
        public ActionResult<UserQueryPaginationResult> UserPaginationQuery([FromQuery] UserQueryPagination request)
        {
            try
            {
                return Ok(_userService.QueryLDAPUsersWithPagination(request));
            }
            catch (HttpResponseException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }

        [HttpGet("Student/ActiveQuestionnaires")]
        [Authorize(AuthenticationSchemes = "AccessToken")]
        [ProducesResponseType(typeof(List<ActiveQuestionnaireKeysetPaginationResultStudent>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ActiveQuestionnaireKeysetPaginationResultStudent>>> GetActiveQuestionnairesForStudent(ActiveQuestionnaireKeysetPaginationRequestStudent request)
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
            
            return Ok(await _userService.GetActiveQuestionnairesForStudent(request, userId));
        }

        [HttpGet("Teacher/ActiveQuestionnaires")]
        [Authorize(AuthenticationSchemes = "AccessToken")]
        [ProducesResponseType(typeof(List<ActiveQuestionnaireKeysetPaginationResultTeacher>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ActiveQuestionnaireKeysetPaginationResultTeacher>>> GetActiveQuestionnairesForTeacher(ActiveQuestionnaireKeysetPaginationRequestTeacher request)
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
            
            return Ok(await _userService.GetActiveQuestionnairesForTeacher(request, userId));
        }
    }
}
