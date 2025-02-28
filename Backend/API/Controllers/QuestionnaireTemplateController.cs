using API.DTO.Requests.QuestionnaireTemplate;
using API.Exceptions;
using API.Services;
using Database.DTO.QuestionnaireTemplate;
using Database.Interfaces;
using Microsoft.AspNetCore.Mvc;
using API.DTO.Responses.QuestionnaireTemplate;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionnaireTemplateController : ControllerBase
    {
        private readonly QuestionnaireTemplateService _questionnaireTemplateService;

        public QuestionnaireTemplateController(IQuestionnaireTemplateRepository questionnaireTemplateRepository, QuestionnaireTemplateService questionnaireTemplateService)
        {
            _questionnaireTemplateService = questionnaireTemplateService;
        }

        /// <summary>
        /// Retrieves a list of questionnaire templates based on the specified request parameters.
        /// </summary>
        /// <param name="request">The request parameters for retrieving questionnaire templates.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a list of <see cref="KeysetPaginationResult"/>.
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(KeysetPaginationResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<KeysetPaginationResult>> GetQuestionnaireTemplates([FromQuery] KeysetPaginationQuery request)
        {
            return Ok(await _questionnaireTemplateService.GetTemplateBasesWithKeysetPagination(request));
        }

        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(FetchTemplate), StatusCodes.Status201Created)]
        public async Task<ActionResult<FetchTemplate>> AddQuestionnaireTemplate([FromBody] AddTemplate questionnaireTemplate)
        {
            FetchTemplate template;
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(FetchTemplate), StatusCodes.Status200OK)]
        public async Task<ActionResult<FetchTemplate>> GetQuestionnaireTemplate(Guid id)
        {
            FetchTemplate template;
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(FetchTemplate), StatusCodes.Status200OK)]
        public async Task<ActionResult<FetchTemplate>> UpdateQuestionnaireTemplate(Guid id, [FromBody] UpdateTemplate updateRequest)
        {
            FetchTemplate updatedTemplate;
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(FetchTemplate), StatusCodes.Status200OK)]
        public async Task<ActionResult<FetchTemplate>> PatchQuestionnaireTemplate(Guid id, [FromBody] QuestionnaireTemplatePatch patchRequest)
        {
            FetchTemplate patchedTemplate;
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