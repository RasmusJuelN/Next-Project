using API.DTO.LDAP;
using API.DTO.Requests.ActiveQuestionnaire;
using API.DTO.Requests.User;
using API.DTO.Responses.ActiveQuestionnaire;
using API.DTO.Responses.User;
using API.Exceptions;
using API.Extensions;
using API.Services;
using Database.DTO;
using Database.DTO.ActiveQuestionnaire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

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
        // test to see the list of all studentname from individual class 
        // Endpoint: GET /api/User/Groups/h1/Students
        [HttpGet("Groups/{groupName}/Students")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public ActionResult<List<LdapUserDTO>> GetStudentsInGroup(string groupName)
        {
            try
            {
                var students = _userService.GetStudentsInGroup(groupName);
                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching students: {ex.Message}");
            }
        }

        //[HttpGet("Search")]
        //[Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        //public ActionResult<UserPaginationResult> SearchStudents([FromQuery] string term, [FromQuery] int pageSize = 10)
        //{
        //    var result = _userService.SearchStudentsAcrossGroups(term, pageSize, null);
        //    return Ok(result);
        //}

        [HttpGet("Groups/{groupName}/StudentsGrouped")]
        public ActionResult<ClassStudentsDTO> GetStudentsGrouped(string groupName)
        {
            try
            {
                var students = _userService.GetStudentsInGroup(groupName);
                var grouped = _userService.GetStudentsGrouped(students);
                return Ok(grouped);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching grouped students: {ex.Message}");
            }
        }
        //[HttpGet("Groups/Classes")]
        //[Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        //public ActionResult<List<string>> GetAllClasses()
        //{
        //    try
        //    {
        //        var classes = _userService.GetAllClassNames();
        //        return Ok(classes); 
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error fetching class names: {ex.Message}");
        //    }
        //}

        [HttpGet("classes")]
        public IActionResult GetClasses()
        {
            var classes = _userService.GetClassesWithStudentRole();
            return Ok(classes);
        }
        //[HttpGet("SearchDynamic")]
        //[Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        //public async Task<ActionResult<List<string>>> SearchDynamic([FromQuery] string term, [FromQuery] string type)
        //{
        //    try
        //    {
        //        var results = await _userService.SearchClassesOrTeachers(term, type);
        //        return Ok(results);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error searching {type}: {ex.Message}");
        //    }
        //}




    }
}
