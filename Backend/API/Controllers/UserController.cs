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
    /// <summary>
    /// Provides endpoints for user management and user-specific data retrieval operations.
    /// Handles LDAP user queries, active questionnaire access for students and teachers,
    /// and role-specific functionality within the application.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserService userService) : ControllerBase
    {
        private readonly UserService _userService = userService;

        /// <summary>
        /// Retrieves a paginated list of users from the LDAP directory with optional filtering.
        /// This endpoint allows administrators to search and browse through user accounts
        /// with support for pagination to handle large result sets efficiently.
        /// </summary>
        /// <param name="request">
        /// The pagination and filtering parameters for the user query.
        /// Contains page size, session identifiers, search criteria, and role filters.
        /// </param>
        /// <returns>
        /// A paginated result containing user information, pagination metadata,
        /// and continuation tokens for retrieving subsequent pages.
        /// </returns>
        /// <response code="200">
        /// Returns the paginated user query results successfully.
        /// The response includes user data, current page information, and navigation tokens.
        /// </response>
        /// <response code="401">Unauthorized - Invalid or missing access token.</response>
        /// <response code="403">Forbidden - User does not have administrative privileges.</response>
        /// <response code="500">Internal server error - LDAP connection issues or server errors.</response>
        /// <remarks>
        /// Requires administrator privileges. This endpoint queries the LDAP directory
        /// and may involve network calls to external authentication systems.
        /// Use the returned session identifiers to navigate through multiple pages of results.
        /// </remarks>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminAndTeacherOnly")]
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

        /// <summary>
        /// Retrieves a paginated list of active questionnaires assigned to the authenticated student.
        /// This endpoint allows students to view questionnaires they need to complete,
        /// with support for pagination and filtering based on questionnaire status.
        /// </summary>
        /// <param name="request">
        /// The keyset pagination parameters for filtering and navigating through active questionnaires.
        /// Includes page size, cursor information, and optional filtering criteria.
        /// </param>
        /// <returns>
        /// A list of active questionnaires formatted specifically for student consumption,
        /// </returns>
        /// <response code="200">
        /// Returns the list of active questionnaires for the student successfully.
        /// Each questionnaire includes metadata relevant to student completion workflow.
        /// </response>
        /// <response code="401">
        /// Unauthorized - Invalid access token, missing token, or user ID extraction failed.
        /// This can occur if the JWT token is malformed or doesn't contain the required subject claim.
        /// </response>
        /// <response code="403">Forbidden - User does not have student privileges.</response>
        /// <response code="500">Internal server error - Database connectivity issues or server errors.</response>
        /// <remarks>
        /// Requires student role privileges. The user ID is automatically extracted from the JWT token's
        /// subject claim. This endpoint uses keyset pagination for efficient navigation through
        /// large collections of questionnaires.
        /// </remarks>
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

        /// <summary>
        /// Retrieves all pending active questionnaires for the authenticated student.
        /// This endpoint provides a focused view of questionnaires that require immediate attention,
        /// filtering out completed, expired, or future questionnaires to show only actionable items.
        /// </summary>
        /// <returns>
        /// A list of pending active questionnaires specifically formatted for student view,
        /// containing essential information needed for questionnaire completion workflow.
        /// </returns>
        /// <response code="200">
        /// Returns the list of pending questionnaires successfully.
        /// </response>
        /// <response code="401">
        /// Unauthorized - Invalid access token, missing token, or failed user ID extraction.
        /// This occurs when the JWT token is missing, expired, or doesn't contain a valid subject claim.
        /// </response>
        /// <response code="403">Forbidden - User does not have student privileges.</response>
        /// <response code="500">Internal server error - Database connectivity issues or server errors.</response>
        /// <remarks>
        /// Requires student role privileges. The user ID is automatically extracted from the JWT token.
        /// This endpoint filters questionnaires to show only those in a "pending" state,
        /// making it ideal for dashboard or notification scenarios where students need
        /// to see what requires their immediate attention.
        /// </remarks>
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

        /// <summary>
        /// Retrieves a paginated list of active questionnaires for the authenticated teacher.
        /// This endpoint provides a focused view of questionnaires that require immediate attention,
        /// filtering out completed, expired, or future questionnaires to show only actionable items,
        /// with comprehensive pagination support and teacher-specific metadata.
        /// </summary>
        /// <param name="request">
        /// The keyset pagination parameters for navigating through the teacher's questionnaires.
        /// Includes page size, navigation cursors, and optional filtering criteria.
        /// </param>
        /// <returns>
        /// A list of active questionnaires formatted for teacher consumption,
        /// containing essential information needed for questionnaire completion workflow, including the student.
        /// </returns>
        /// <response code="200">
        /// Returns the paginated list of teacher's questionnaires successfully.
        /// </response>
        /// <response code="401">
        /// Unauthorized - Invalid access token, missing token, or user ID extraction failed.
        /// This can occur if the JWT token is malformed or doesn't contain the required subject claim.
        /// </response>
        /// <response code="403">Forbidden - User does not have teacher privileges.</response>
        /// <response code="500">Internal server error - Database connectivity issues or server errors.</response>
        /// <remarks>
        /// Requires teacher role privileges. The user ID is automatically extracted from the JWT token's
        /// subject claim. This endpoint provides teacher-specific views including student participation
        /// data.
        /// </remarks>
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

        /// <summary>
        /// Retrieves all pending active questionnaires managed by the authenticated teacher.
        /// This endpoint provides teachers with a focused view of questionnaires requiring attention,
        /// such as those with approaching deadlines, low participation rates, or pending reviews.
        /// </summary>
        /// <returns>
        /// A list of pending active questionnaires formatted for teacher management,
        /// containing administrative metadata, participation statistics, and actionable insights.
        /// </returns>
        /// <response code="200">
        /// Returns the list of pending teacher questionnaires successfully.
        /// Each questionnaire includes management metadata, student participation data, and status information.
        /// </response>
        /// <response code="401">
        /// Unauthorized - Invalid access token, missing token, or failed user ID extraction.
        /// This occurs when the JWT token is missing, expired, or doesn't contain a valid subject claim.
        /// </response>
        /// <response code="403">Forbidden - User does not have teacher privileges.</response>
        /// <response code="500">Internal server error - Database connectivity issues or server errors.</response>
        /// <remarks>
        /// Requires teacher role privileges. The user ID is automatically extracted from the JWT token.
        /// This endpoint filters questionnaires to show only those requiring teacher attention,
        /// making it suitable for administrative dashboards where teachers need to prioritize
        /// their questionnaire management tasks.
        /// </remarks>
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
        //[HttpGet("Groups/{groupName}/Students")]
        //[Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        //public ActionResult<List<LdapUserDTO>> GetStudentsInGroup(string groupName)
        //{
        //    try
        //    {
        //        var students = _userService.GetStudentsInGroup(groupName);
        //        return Ok(students);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error fetching students: {ex.Message}");
        //    }
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


        [HttpGet("classes")]
        public IActionResult GetClasses()
        {
            var classes = _userService.GetClassesWithStudentRole();
            return Ok(classes);
        }



    }
}
