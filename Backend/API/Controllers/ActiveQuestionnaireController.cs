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
    /// <summary>
    /// Controller for managing active questionnaires in the system.
    /// Provides endpoints for activating questionnaires, retrieving questionnaire data,
    /// submitting answers, and checking questionnaire status.
    /// </summary>
    /// <remarks>
    /// This controller handles various operations related to active questionnaires:
    /// <list type="bullet">
    /// <item>Fetching paginated lists of active questionnaires (admin only)</item>
    /// <item>Activating questionnaire templates (admin only)</item>
    /// <item>Checking if users have active questionnaires</item>
    /// <item>Retrieving specific questionnaire details</item>
    /// <item>Submitting questionnaire answers</item>
    /// <item>Getting questionnaire responses (teacher only)</item>
    /// <item>Checking if questionnaires are answered or completed</item>
    /// </list>
    /// 
    /// All endpoints require authentication via AccessToken scheme.
    /// Different authorization policies are applied based on user roles (Admin, Teacher, Student).
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class ActiveQuestionnaireController : ControllerBase
    {
        private readonly ActiveQuestionnaireService _questionnaireService;
        private readonly ILogger _logger;

        public ActiveQuestionnaireController(ActiveQuestionnaireService questionnaireService, ILoggerFactory loggerFactory)
        {
            _questionnaireService = questionnaireService;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        /// <summary>
        /// Retrieves a paginated list of active questionnaires with administrative details.
        /// </summary>
        /// <param name="request">The pagination request containing filters and pagination parameters for fetching active questionnaires.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing an <see cref="ActiveQuestionnaireKeysetPaginationResultAdmin"/> 
        /// with the paginated list of active questionnaires and pagination metadata.
        /// </returns>
        /// <remarks>
        /// This endpoint requires authentication using an access token. Only authenticated users can access this resource.
        /// The response includes administrative information about the questionnaires.
        /// </remarks>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AccessToken")]
        public async Task<ActionResult<ActiveQuestionnaireKeysetPaginationResultAdmin>> GetActiveQuestionnaires([FromQuery] ActiveQuestionnaireKeysetPaginationRequestFull request)
        {
            return Ok(await _questionnaireService.FetchActiveQuestionnaireBases(request));
        }


        /// <summary>
        /// Activates a questionnaire template by creating an active questionnaire instance.
        /// </summary>
        /// <param name="request">The activation request containing the questionnaire template details and activation parameters.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing the newly created <see cref="ActiveQuestionnaire"/> if successful,
        /// or an appropriate error response if the activation fails.
        /// </returns>
        /// <response code="200">Returns the activated questionnaire instance.</response>
        /// <response code="400">If the request data is invalid or the questionnaire template cannot be activated.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have admin privileges.</response>
        [HttpPost("activate")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<List<ActiveQuestionnaire>>> ActivateQuestionnaire([FromBody] ActivateQuestionnaire request)
        {
            // This should return a list of created questionnaires, one for each student/teacher combination
            var result = await _questionnaireService.ActivateTemplate(request);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a paginated list of questionnaire groups using keyset pagination.
        /// </summary>
        /// <param name="request">
        /// The <see cref="QuestionnaireGroupKeysetPaginationRequest"/> containing pagination,
        /// ordering, and optional filtering parameters.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult{QuestionnaireGroupKeysetPaginationResult}"/> containing
        /// the requested page of questionnaire groups, or an error response if an exception occurs.
        /// </returns>
        /// <remarks>
        /// This endpoint requires the user to be authenticated as an Admin. 
        /// Errors are logged and return HTTP 500 if an exception occurs.
        /// </remarks>
        [HttpGet("groups/paginated")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<QuestionnaireGroupKeysetPaginationResult>> GetGroupsPaginated([FromQuery] QuestionnaireGroupKeysetPaginationRequest request)
        {
            try
            {
                var result = await _questionnaireService.FetchQuestionnaireGroupsWithKeysetPagination(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching paginated questionnaire groups: {Message}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Creates a new questionnaire group from a specified template and assigns participants.
        /// </summary>
        /// <param name="request">
        /// The <see cref="ActivateQuestionnaireGroup"/> object containing the group name, template ID,
        /// and lists of student and teacher GUIDs to include.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult{QuestionnaireGroupResult}"/> containing the created group details,
        /// or an error response if an exception occurs.
        /// </returns>
        /// <remarks>
        /// This endpoint requires the user to be authenticated as an Admin.
        /// Errors are logged and return HTTP 500 if an exception occurs.
        /// </remarks>
        [HttpPost("createGroup")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<QuestionnaireGroupResult>> CreateGroup([FromBody] ActivateQuestionnaireGroup request)
        {
            try
            {
                var result = await _questionnaireService.ActivateQuestionnaireGroup(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating questionnaire group: {Message}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Retrieves all questionnaire groups including their active questionnaires and participants.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{List{QuestionnaireGroupResult}}"/> containing all questionnaire groups,
        /// or an error response if an exception occurs.
        /// </returns>
        /// <remarks>
        /// This endpoint requires the user to be authenticated as an Admin.
        /// Errors are logged and return HTTP 500 if an exception occurs.
        /// </remarks>
        [HttpGet("groups")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<List<QuestionnaireGroupResult>>> GetAllGroups()
        {
            try
            {
                var results = await _questionnaireService.GetAllQuestionnaireGroups();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all questionnaire groups: {Message}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Retrieves basic information for all questionnaire groups.
        /// </summary>
        /// <returns>
        /// A list of <see cref="QuestionnaireGroupBasicResult"/> containing basic information about all questionnaire groups.
        /// Returns HTTP 200 with the list on success, or HTTP 500 with error message on failure.
        /// </returns>
        /// <remarks>
        /// This endpoint requires admin authorization and uses access token authentication.
        /// </remarks>
        [HttpGet("groupsBasic")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<List<QuestionnaireGroupBasicResult>>> GetAllGroupsBasic()
        {
            try
            {
                var results = await _questionnaireService.GetAllQuestionnaireGroupsBasic();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all questionnaire groups: {Message}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Retrieves detailed information about a single questionnaire group by its ID.
        /// </summary>
        /// <param name="groupId">The GUID of the questionnaire group to retrieve.</param>
        /// <returns>
        /// An <see cref="ActionResult{QuestionnaireGroupResult}"/> containing the group details if found,
        /// <c>NotFound()</c> if the group does not exist, or an error response if an exception occurs.
        /// </returns>
        /// <remarks>
        /// This endpoint requires the user to be authenticated. Admin access is not strictly required.
        /// Errors are logged and return HTTP 500 if an exception occurs.
        /// </remarks>
        [HttpGet("{groupId}/getGroup")]
        [Authorize(AuthenticationSchemes = "AccessToken")]
        public async Task<ActionResult<QuestionnaireGroupResult>> GetGroup(Guid groupId)
        {
            try
            {
                var result = await _questionnaireService.GetQuestionnaireGroup(groupId);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching questionnaire group: {Message}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Checks if the authenticated user has an active questionnaire.
        /// </summary>
        /// <returns>
        /// Returns the GUID of the oldest active questionnaire if one exists for the user,
        /// or 204 No Content if no active questionnaire is found.
        /// </returns>
        /// <response code="200">Returns the GUID of the oldest active questionnaire for the user.</response>
        /// <response code="204">No active questionnaire found for the user.</response>
        /// <response code="401">Unauthorized - invalid or missing authentication token, or error parsing user ID from claims.</response>
        /// <remarks>
        /// This endpoint requires authentication with an access token and is restricted to users with Student or Teacher roles.
        /// The user ID is extracted from the JWT token's 'sub' claim.
        /// </remarks>
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
            catch (Exception e)
            {
                _logger.LogError(e, "Error parsing user ID from claims: {Message}", e.Message);
                return Unauthorized();
            }

            return await _questionnaireService.GetOldestActiveQuestionnaireForUser(userId);
        }

        /// <summary>
        /// Retrieves an active questionnaire by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the active questionnaire to retrieve.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing the <see cref="ActiveQuestionnaire"/> if found.
        /// Returns HTTP 200 (OK) with the questionnaire data on success.
        /// </returns>
        /// <remarks>
        /// This endpoint requires authentication using the AccessToken scheme.
        /// </remarks>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "AccessToken")]
        public async Task<ActionResult<ActiveQuestionnaire>> GetActiveQuestionnaire(Guid id)
        {
            return Ok(await _questionnaireService.FetchActiveQuestionnaire(id));
        }

        /// <summary>
        /// Submits an answer submission for a specific questionnaire.
        /// </summary>
        /// <param name="id">The unique identifier of the questionnaire to submit answers for.</param>
        /// <param name="submission">The answer submission data containing the user's responses.</param>
        /// <returns>
        /// Returns an <see cref="ActionResult"/> indicating the result of the operation:
        /// - 200 OK if the answers were successfully submitted
        /// - 401 Unauthorized if the user claims cannot be parsed or user is not authenticated
        /// - Various status codes based on the specific error returned by the questionnaire service
        /// </returns>
        /// <remarks>
        /// This endpoint requires authentication with an AccessToken and the user must have either Student or Teacher role.
        /// The user ID is extracted from the JWT claims and used to associate the submission with the authenticated user.
        /// </remarks>
        [HttpPut("{id}/submitAnswer")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "StudentAndTeacherOnly")]
        public async Task<ActionResult> SubmitQuestionnaireAnswer(Guid id, [FromBody] AnswerSubmission submission)
        {
            Guid userId;
            try
            {
                userId = Guid.Parse(User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error parsing user ID from claims: {Message}", e.Message);
                return Unauthorized();
            }

            try
            {
                await _questionnaireService.SubmitAnswers(id, userId, submission);
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(ex, "Error submitting questionnaire answer: {Message}", ex.Message);
                return StatusCode((int)ex.StatusCode, ex.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Retrieves the responses for a specific active questionnaire.
        /// </summary>
        /// <param name="id">The unique identifier of the questionnaire to get responses for.</param>
        /// <returns>
        /// A list of <see cref="FullResponse"/> objects containing all responses for the specified questionnaire.
        /// Returns <see cref="UnauthorizedResult"/> if the user is not authorized to view the responses.
        /// </returns>
        /// <remarks>
        /// This endpoint requires teacher authorization and validates that the requesting user 
        /// is either the student who submitted the response or the teacher associated with it.
        /// </remarks>
        /// <response code="200">Returns the questionnaire responses successfully.</response>
        /// <response code="401">User is not authorized to view the questionnaire responses.</response>
        [HttpGet("{id}/getResponse")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "TeacherOnly")]
        public async Task<ActionResult<List<FullResponse>>> GetActiveQuestionnaireResponses(Guid id)
        {
            Guid userId;
            try
            {
                userId = Guid.Parse(User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error parsing user ID from claims: {Message}", e.Message);
                return Unauthorized();
            }

            FullResponse response = await _questionnaireService.GetFullResponseAsync(id);

            if (userId != response.Student.User.Guid && userId != response.Teacher.User.Guid)
            {
                _logger.LogWarning("User {UserId} is not authorized to view questionnaire response for {QuestionnaireId}", userId, id);
                return Unauthorized();
            }
            else
            {
                return Ok(await _questionnaireService.GetFullResponseAsync(id));
            }
        }

        /// <summary>
        /// Checks if the authenticated user has already answered a specific questionnaire.
        /// </summary>
        /// <param name="id">The unique identifier of the questionnaire to check.</param>
        /// <returns>
        /// A boolean value indicating whether the user has submitted an answer for the questionnaire.
        /// Returns true if the user has answered the questionnaire, false otherwise.
        /// </returns>
        /// <response code="200">Returns true if the user has answered the questionnaire, false otherwise.</response>
        /// <response code="401">Returned when the user is not authenticated or the user ID cannot be parsed from claims.</response>
        /// <response code="403">Returned when the user is not authorized (must be a student or teacher).</response>
        [HttpGet("{id}/isAnswered")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "StudentAndTeacherOnly")]
        public async Task<ActionResult<bool>> CheckIfQuestionnaireAnswered(Guid id)
        {
            Guid userId;
            try
            {
                userId = Guid.Parse(User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error parsing user ID from claims: {Message}", e.Message);
                return Unauthorized();
            }

            return await _questionnaireService.HasUserSubmittedAnswer(userId, id);
        }

        /// <summary>
        /// Checks if a specific questionnaire has been completed by the authenticated teacher.
        /// </summary>
        /// <param name="id">The unique identifier of the questionnaire to check completion status for.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains:
        /// - <see cref="ActionResult{T}"/> where T is <see cref="bool"/>
        /// - Returns true if the questionnaire is completed, false otherwise
        /// - Returns <see cref="UnauthorizedResult"/> if the user ID cannot be parsed from claims
        /// </returns>
        /// <remarks>
        /// This endpoint requires teacher authorization and uses access token authentication.
        /// The method extracts the user ID from JWT claims and delegates the completion check to the questionnaire service.
        /// </remarks>
        /// <response code="200">Returns boolean indicating completion status</response>
        /// <response code="401">Returned when user authentication fails or user ID cannot be parsed</response>
        /// <response code="403">Returned when user is not authorized as a teacher</response>
        [HttpGet("{id}/IsCompleted")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "TeacherOnly")]
        public async Task<ActionResult<bool>> CheckifQuestionnaireCompleted(Guid id)
        {
            Guid userId;
            try
            {
                userId = Guid.Parse(User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error parsing user ID from claims: {Message}", e.Message);
                return Unauthorized();
            }

            return await _questionnaireService.IsActiveQuestionnaireComplete(id, userId);
        }


        [HttpGet("{studentid},{templateid}/getResponsesFromUserAndTemplate")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "TeacherOnly")]
        public async Task<ActionResult<List<FullResponse>>> GetResponsesFromTemplatesAndStudent(Guid studentid, Guid templateid)
        {
            Guid userId;
            try
            {
                userId = Guid.Parse(User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error parsing user ID from claims: {Message}", e.Message);
                return Unauthorized();
            }

            List<FullStudentRespondsDate> response = await _questionnaireService.GetResponsesFromStudentAndTemplateAsync(studentid, templateid);

            if (response.Count > 0)
            {
                if (userId != response[0].Student.User.Guid)
                {
                    _logger.LogWarning("User {UserId} is not authorized to view questionnaire response for {QuestionnaireId}", userId, studentid);
                    return Unauthorized();
                }
                else
                {
                    return Ok(await _questionnaireService.GetResponsesFromStudentAndTemplateAsync(studentid, templateid));
                }
            }
            else { return Ok(await _questionnaireService.GetResponsesFromStudentAndTemplateAsync(studentid, templateid)); }
        }

        [HttpGet("{studentid},{templateid}/getResponsesFromUserAndTemplateWithDate")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "TeacherOnly")]
        public async Task<ActionResult<List<FullResponse>>> GetResponsesFromTemplatesAndStudentWithDate(Guid studentid, Guid templateid)
        {
            Guid userId;
            try
            {
                userId = Guid.Parse(User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error parsing user ID from claims: {Message}", e.Message);
                return Unauthorized();
            }

            List<FullStudentRespondsDate> response = await _questionnaireService.GetResponsesFromStudentAndTemplateWithDateAsync(studentid, templateid);

            return Ok(await _questionnaireService.GetResponsesFromStudentAndTemplateWithDateAsync(studentid, templateid));
        }


        /// <summary>
        /// Retrieves anonymised survey responses for a specific questionnaire.
        /// </summary>
        /// <param name="responsesRequest">The request containing questionnaire ID, users, and groups to filter responses.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a <see cref="SurveyResponseSummary"/> with anonymised response data,
        /// or a 500 status code with error message if an exception occurs.
        /// </returns>
        /// <remarks>
        /// This endpoint requires teacher authorization and uses access token authentication.
        /// The response data is anonymised to protect user privacy while providing survey insights.
        /// </remarks>
        [HttpGet("GetAnonymisedResponses/")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "TeacherOnly")]
        public async Task<ActionResult<SurveyResponseSummary>> GetAnonymisedResponses([FromQuery] AnonymisedResponsesRequest responsesRequest)
        {
            try
            {
                SurveyResponseSummary result = await _questionnaireService.GetAnonymisedResponses(responsesRequest.QuestionnaireId, responsesRequest.Users, responsesRequest.Groups);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching anonymised responses: {Message}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
