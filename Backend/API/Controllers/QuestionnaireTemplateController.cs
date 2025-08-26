using API.DTO.Requests.QuestionnaireTemplate;
using API.Exceptions;
using API.Services;
using Database.DTO.QuestionnaireTemplate;
using Microsoft.AspNetCore.Mvc;
using API.DTO.Responses.QuestionnaireTemplate;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
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
        /// Retrieves a list of questionnaire templates based on the specified request parameters.
        /// </summary>
        /// <param name="request">The request parameters for retrieving questionnaire templates.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a list of <see cref="TemplateKeysetPaginationResult"/>.
        /// </returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Get /api/questionnaire-template?PageSize=5&amp;Order=CreatedAtDesc
        ///     
        /// <para></para>
        /// queryCursor is a unique combination of the lastUpdated and Id values of the last item in the result.
        /// For subsequent queries, this can be included to indicate and control where the query should resume from.
        /// </remarks>
        /// <response code="200">Returns the list of questionnaire templates.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminAndTeacherOnly")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TemplateKeysetPaginationResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<TemplateKeysetPaginationResult>> GetQuestionnaireTemplates([FromQuery] TemplateKeysetPaginationRequest request)
        {
            return Ok(await _questionnaireTemplateService.GetTemplateBasesWithKeysetPagination(request));
        }

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

            return Ok(updatedTemplate);
        }

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
    }
}