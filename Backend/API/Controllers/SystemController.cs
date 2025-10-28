using API.DTO.Requests.Settings;
using API.DTO.Responses.Settings;
using API.DTO.Responses.Settings.SettingsSchema;
using API.Services;
using Database.DTO.ApplicationLog;
using Database.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Settings.Default;
using Settings.Models;

namespace API.Controllers
{
    /// <summary>
    /// Controller responsible for system-level operations and health monitoring endpoints.
    /// </summary>
    /// <remarks>
    /// The SystemController provides essential system functionality including health checks
    /// and monitoring capabilities that are typically used by infrastructure components
    /// such as load balancers, container orchestrators, and monitoring systems.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController(IApplicationLogRepository ApplicationLogsRepository, SystemControllerService systemControllerService) : ControllerBase
    {
        private readonly IApplicationLogRepository _ApplicationLogsRepository = ApplicationLogsRepository;
        private readonly SystemControllerService _SystemControllerService = systemControllerService;

        /// <summary>
        /// Health check endpoint that responds to HEAD requests to verify the API is running.
        /// </summary>
        /// <returns>An HTTP 200 OK response if the service is healthy and available.</returns>
        /// <remarks>
        /// This endpoint is typically used by load balancers, monitoring systems, or other services
        /// to perform lightweight health checks without returning any response body.
        /// </remarks>
        [HttpHead("ping")]
        [AllowAnonymous]
        public IActionResult Ping()
        {
            return Ok();
        }

        /// <summary>
        /// Retrieves application logs from the database based on the specified query parameters.
        /// </summary>
        /// <param name="logQuery">The query parameters to filter and paginate the application logs.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a list of <see cref="ApplicationLog"/> objects
        /// that match the specified query criteria.
        /// </returns>
        /// <remarks>
        /// This endpoint requires admin-level authorization with an access token.
        /// The logs are retrieved asynchronously from the application logs table in the database.
        /// </remarks>
        [HttpGet("logs/db")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<List<ApplicationLog>>> GetDatabaseLogs([FromQuery] ApplicationLogQuery logQuery)
        {
            return Ok(await _ApplicationLogsRepository.GetApplicationLogsAsync(logQuery));
        }

        /// <summary>
        /// Retrieves all available log categories from the database.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a string representation of the log categories.
        /// Returns HTTP 200 (OK) with the log categories data on success.
        /// </returns>
        /// <remarks>
        /// This endpoint requires admin-level authorization and uses access token authentication.
        /// </remarks>
        [HttpGet("logs/db/categories")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<string>> GetDatabaseLogCategories()
        {
            return Ok(await _ApplicationLogsRepository.GetLogCategoriesAsync());
        }

        /// <summary>
        /// Retrieves a list of EventIds that correspond to log events stored in the database.
        /// This endpoint is restricted to administrators only.
        /// </summary>
        /// <returns>
        /// An ActionResult containing a list of EventId objects that match the event IDs 
        /// found in the database logs. Returns an empty list if no matching events are found.
        /// </returns>
        /// <remarks>
        /// This method performs the following operations:
        /// 1. Retrieves all log event IDs from the database via the repository
        /// 2. Uses reflection to discover all types that inherit from LogEventsBase across all loaded assemblies
        /// 3. Extracts all static EventId fields from the discovered event types
        /// 4. Filters the EventIds to only include those that have corresponding entries in the database
        /// 
        /// The method handles ReflectionTypeLoadException gracefully by ignoring assemblies that cannot be loaded.
        /// </remarks>
        /// <response code="200">Returns the list of matching EventIds</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not authorized (not an admin)</response>
        [HttpGet("logs/db/events")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<List<EventId>>> GetDatabaseLogEvents()
        {
            return Ok(await _ApplicationLogsRepository.GetLogEventsAsync());
        }

        [HttpGet("logs/file/{filename}")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<FileResult> GetLogFile(string filename)
        {
            return await _SystemControllerService.GetLogFile(filename);
        }

        [HttpGet("logs/file/list")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<List<string>>> GetLogFileNames()
        {
            return Ok(_SystemControllerService.GetLogFileNames());
        }

        [HttpGet("settings")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<SettingsFetchResponse>> GetSettings()
        {
            return Ok(await _SystemControllerService.GetSettings());
        }

        [HttpGet("settings/schema")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<SettingsSchema>> GetSettingsSchema()
        {
            return Ok(await _SystemControllerService.GetSettingsSchema());
        }

        [HttpGet("settings/default")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<DefaultSettings>> GetDefaultSettings()
        {
            return Ok(new DefaultSettings());
        }

        [HttpPut("settings/update")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateSettingsRequest settings)
        {
            bool result = await _SystemControllerService.UpdateSettings(settings);

            if (result == false)
            {
                return BadRequest("Failed to update settings.");
            }

            return Ok();
        }

        [HttpPatch("settings/patch")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<IActionResult> PatchSettings([FromBody] PatchSettingsRequest settings)
        {
            bool result = await _SystemControllerService.PatchSettings(settings);

            if (result == false)
            {
                return BadRequest("Failed to patch settings.");
            }

            return Ok();
        }
    }
}
