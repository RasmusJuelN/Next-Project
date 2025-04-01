using System.IdentityModel.Tokens.Jwt;
using API.DTO.Requests.ActiveQuestionnaire;
using API.DTO.Requests.User;
using API.DTO.Responses.ActiveQuestionnaire;
using API.DTO.Responses.User;
using API.Exceptions;
using API.Extensions;
using API.Services;
using Database.DTO.ActiveQuestionnaire;
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
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
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
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "StudentOnly")]
        [ProducesResponseType(typeof(List<ActiveQuestionnaireKeysetPaginationResultStudent>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ActiveQuestionnaireKeysetPaginationResultStudent>>> GetActiveQuestionnairesForStudent([FromQuery]ActiveQuestionnaireKeysetPaginationRequestStudent request)
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

        [HttpGet("Student/ActiveQuestionnaires/Pending")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "StudentOnly")]
        [ProducesResponseType(typeof(List<ActiveQuestionnaireStudentBase>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ActiveQuestionnaireStudentBase>>> GetPendingActiveQuestionnairesForStudent()
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

            List<ActiveQuestionnaireBase> activeQuestionnaireBases = await _userService.GetPendingActiveQuestionnaires(userId);

            return Ok(activeQuestionnaireBases.Select(a => a.ToActiveQuestionnaireStudentDTO()).ToList());
        }

        [HttpGet("Teacher/ActiveQuestionnaires")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "TeacherOnly")]
        [ProducesResponseType(typeof(List<ActiveQuestionnaireKeysetPaginationResultTeacher>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ActiveQuestionnaireKeysetPaginationResultTeacher>>> GetActiveQuestionnairesForTeacher([FromQuery]ActiveQuestionnaireKeysetPaginationRequestTeacher request)
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

        [HttpGet("Teacher/ActiveQuestionnaires/Pending")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "TeacherOnly")]
        [ProducesResponseType(typeof(List<ActiveQuestionnaireTeacherBase>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ActiveQuestionnaireTeacherBase>>> GetPendingActiveQuestionnairesForTeacher()
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

            List<ActiveQuestionnaireBase> activeQuestionnaireBases = await _userService.GetPendingActiveQuestionnaires(userId);

            return Ok(activeQuestionnaireBases.Select(a => a.ToActiveQuestionnaireTeacherDTO()).ToList());
        }
    }
}
