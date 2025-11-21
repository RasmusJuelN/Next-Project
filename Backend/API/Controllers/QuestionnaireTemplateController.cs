using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using API.DTO.Requests.QuestionnaireTemplate;
using API.DTO.Responses.QuestionnaireTemplate;
using API.Exceptions;
using API.Services;
using Database.DTO.QuestionnaireTemplate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Controller responsible for managing questionnaire templates in the system.
    /// Provides CRUD operations and pagination functionality for questionnaire templates.
    /// </summary>
    /// <remarks>
    /// This controller handles all operations related to questionnaire templates including:
    /// - Creating new templates
    /// - Retrieving templates with pagination support
    /// - Updating existing templates (full update and partial patch)
    /// - Deleting templates
    /// 
    /// All endpoints require admin-level authorization using AccessToken authentication.
    /// The controller uses keyset pagination for efficient handling of large datasets.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionnaireTemplateController : ControllerBase
    {
        private readonly QuestionnaireTemplateService _questionnaireTemplateService;

        public QuestionnaireTemplateController(QuestionnaireTemplateService questionnaireTemplateService)
        {
            _questionnaireTemplateService = questionnaireTemplateService;
        }

        /// <summary>
        /// Retrieves questionnaire templates with keyset pagination.
        /// </summary>
        /// <param name="request">The pagination request containing parameters for keyset-based pagination.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an ActionResult with:
        /// - 200 OK: A TemplateKeysetPaginationResult containing the paginated questionnaire templates.
        /// - 500 Internal Server Error: When an unexpected error occurs during processing.
        /// </returns>
        /// <remarks>
        /// This endpoint requires admin-level authorization using the AccessToken authentication scheme.
        /// Keyset pagination is used for efficient traversal of large datasets.
        /// </remarks>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminAndTeacherOnly")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TemplateKeysetPaginationResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<TemplateKeysetPaginationResult>> GetQuestionnaireTemplates([FromQuery] TemplateKeysetPaginationRequest request)
        {
            return Ok(await _questionnaireTemplateService.GetTemplateBasesWithKeysetPagination(request));
        }

        /// <summary>
        /// Adds a new questionnaire template to the system.
        /// </summary>
        /// <param name="questionnaireTemplate">The questionnaire template data to be added.</param>
        /// <returns>
        /// Returns a <see cref="CreatedAtRouteResult"/> with the created <see cref="QuestionnaireTemplate"/> if successful,
        /// or a <see cref="ConflictResult"/> if a template with the same identifier already exists.
        /// </returns>
        /// <response code="201">The questionnaire template was successfully created.</response>
        /// <response code="409">A questionnaire template with the same identifier already exists.</response>
        /// <response code="500">An internal server error occurred while processing the request.</response>
        /// <exception cref="SQLException.ItemAlreadyExists">Thrown when attempting to add a template that already exists.</exception>
        /// <remarks>
        /// This endpoint requires administrator privileges and valid access token authentication.
        /// The template will be validated before being added to the system.
        /// </remarks>
        [HttpPost("add")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(QuestionnaireTemplate), StatusCodes.Status201Created)]
        public async Task<ActionResult<QuestionnaireTemplate>> AddQuestionnaireTemplate([FromBody] QuestionnaireTemplateAdd questionnaireTemplate)
        {
            QuestionnaireTemplate template;
            try
            {
                template = await _questionnaireTemplateService.AddTemplate(questionnaireTemplate);

            }
            catch (SQLException.ItemAlreadyExists)
            {
                return Conflict();
            }

            return CreatedAtRoute("", template.Id, template);
        }

        /// <summary>
        /// Retrieves a questionnaire template by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the questionnaire template to retrieve.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing:
        /// - 200 OK with the <see cref="QuestionnaireTemplate"/> if found
        /// - 404 Not Found if the template does not exist
        /// - 500 Internal Server Error if an unexpected error occurs
        /// </returns>
        /// <remarks>
        /// This endpoint requires admin authorization and uses access token authentication.
        /// </remarks>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(QuestionnaireTemplate), StatusCodes.Status200OK)]
        public async Task<ActionResult<QuestionnaireTemplate>> GetQuestionnaireTemplate(Guid id)
        {
            QuestionnaireTemplate template;
            try
            {
                template = await _questionnaireTemplateService.GetTemplateById(id);

            }
            catch (SQLException.ItemNotFound)
            {
                return NotFound();
            }

            return Ok(template);
        }

        /// <summary>
        /// Updates an existing questionnaire template with a new questionnaire template
        /// </summary>
        /// <param name="id">The unique identifier of the questionnaire template to update.</param>
        /// <param name="updateRequest">The update request containing the new questionnaire template.</param>
        /// <returns>
        /// Returns the updated questionnaire template if successful.
        /// Returns 404 Not Found if the template with the specified ID does not exist.
        /// Returns 500 Internal Server Error if an unexpected error occurs during the update process.
        /// </returns>
        /// <remarks>
        /// This endpoint requires admin-level authorization. The user must be authenticated with a valid access token
        /// and have admin privileges to perform this operation.
        /// </remarks>
        [HttpPut("{id}/update")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(QuestionnaireTemplate), StatusCodes.Status200OK)]
        public async Task<ActionResult<QuestionnaireTemplate>> UpdateQuestionnaireTemplate(Guid id, [FromBody] QuestionnaireTemplateUpdate updateRequest)
        {
            QuestionnaireTemplate updatedTemplate;
            try
            {
                updatedTemplate = await _questionnaireTemplateService.UpdateTemplate(id, updateRequest);

            }
            catch (SQLException.ItemNotFound)
            {
                return NotFound();
            }
            catch (SQLException.NotValidated)
            {
                return BadRequest("Validation failed for the update request.");
            }

            return Ok(updatedTemplate);
        }

        /// <summary>
        /// Patches an existing questionnaire template with the provided updates.
        /// </summary>
        /// <param name="id">The unique identifier of the questionnaire template to patch.</param>
        /// <param name="patchRequest">The patch request containing the fields to update.</param>
        /// <returns>
        /// Returns the updated questionnaire template if successful.
        /// Returns 404 Not Found if the template with the specified ID does not exist.
        /// Returns 500 Internal Server Error if an unexpected error occurs.
        /// </returns>
        /// <remarks>
        /// This endpoint requires admin authorization and uses the AccessToken authentication scheme.
        /// Only non-null fields in the patch request will be updated on the template.
        /// </remarks>
        [HttpPatch("{id}/patch")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(QuestionnaireTemplate), StatusCodes.Status200OK)]
        public async Task<ActionResult<QuestionnaireTemplate>> PatchQuestionnaireTemplate(Guid id, [FromBody] QuestionnaireTemplatePatch patchRequest)
        {
            QuestionnaireTemplate patchedTemplate;
            try
            {
                patchedTemplate = await _questionnaireTemplateService.PatchTemplate(id, patchRequest);
            }
            catch (SQLException.ItemNotFound)
            {
                return NotFound();
            }

            return Ok(patchedTemplate);
        }

        /// <summary>
        /// Deletes a questionnaire template by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the questionnaire template to delete.</param>
        /// <returns>
        /// Returns 204 if the template is successfully deleted.
        /// returns 404 Not Found if the template with the specified ID does not exist.
        /// returns 500 Internal Server Error if an unexpected error occurs.
        /// </returns>
        /// <remarks>
        /// This endpoint requires admin-level authorization and uses access token authentication.
        /// The operation is irreversible once completed.
        /// </remarks>
        [HttpDelete("{id}/delete")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteQuestionnaireTemplate(Guid id)
        {
            try
            {
                await _questionnaireTemplateService.DeleteTemplate(id);
            }
            catch (SQLException.ItemNotFound)
            {
                return NotFound();
            }

            return NoContent();
        }
        [HttpPost("{id}/finalize")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(QuestionnaireTemplate), StatusCodes.Status200OK)]
        public async Task<ActionResult<QuestionnaireTemplate>> FinalizeQuestionnaireTemplate(Guid id)
        {
            try
            {
                var updated = await _questionnaireTemplateService.FinalizeTemplate(id);
                return Ok(updated);
            }
            catch (SQLException.ItemNotFound)
            {
                return NotFound();
            }
            // Optional: if your service throws when already finalized or locked, map to 409:
            // catch (BusinessException.AlreadyFinalized) { return Conflict("Template already finalized."); }
        }

        /// <summary>
        /// Restores a soft deleted questionnaire template by changing its status from Deleted.
        /// </summary>
        /// <param name="id">The unique identifier of the questionnaire template to restore.</param>
        /// <returns>
        /// Returns 200 OK with the restored template if successful.
        /// Returns 404 Not Found if the template with the specified ID does not exist.
        /// Returns 400 Bad Request if the template is not in deleted state.
        /// Returns 500 Internal Server Error if an unexpected error occurs.
        /// </returns>
        /// <remarks>
        /// This endpoint requires admin-level authorization and uses access token authentication.
        /// The template status will be set to Draft unless there are active questionnaires associated with it,
        /// in which case it will be set to Finalized to maintain proper workflow state.
        /// </remarks>
        [HttpPost("{id}/undelete")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        [ProducesResponseType(typeof(QuestionnaireTemplate), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuestionnaireTemplate>> UndeleteQuestionnaireTemplate(Guid id)
        {
            try
            {
                var restored = await _questionnaireTemplateService.UndeleteTemplate(id);
                return Ok(restored);
            }
            catch (SQLException.ItemNotFound)
            {
                return NotFound();
            }
            catch (SQLException.NotValidated ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets questionnaire template bases that have been answered by both a specific student and the current teacher.
        /// This endpoint allows teachers to view shared questionnaire history with individual students.
        /// </summary>
        /// <param name="studentId">The ID of the student user to check shared completion with.</param>
        /// <returns>A list of questionnaire template bases where both the teacher and student have completed responses.</returns>
        /// <response code="200">Returns the list of questionnaire template bases answered by both users.</response>
        /// <response code="401">Unauthorized - Invalid or missing access token.</response>
        /// <response code="403">Forbidden - User does not have teacher privileges.</response>
        /// <remarks>
        /// This endpoint filters questionnaire templates to show only those where both the current teacher
        /// and the specified student have submitted completed responses, useful for result history comparisons.
        /// </remarks>
        [HttpGet("AnsweredByStudent/{studentId}")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "TeacherOnly")]
        [ProducesResponseType(typeof(List<Database.DTO.QuestionnaireTemplate.QuestionnaireTemplateBase>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<Database.DTO.QuestionnaireTemplate.QuestionnaireTemplateBase>>> GetTemplateBasesAnsweredByStudent(Guid studentId)
        {
            Guid teacherId;
            try
            {
                teacherId = Guid.Parse(User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
            }
            catch (Exception)
            {
                return Unauthorized();
            }

            var templates = await _questionnaireTemplateService.GetTemplateBasesAnsweredByStudentAsync(studentId, teacherId);
            return Ok(templates);
        }
    }
}