using Database.DTO.ApplicationLog;
using Database.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Controller for managing application logs and log-related operations.
    /// Provides endpoints for retrieving database logs and log categories.
    /// Requires admin authorization for all operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController(IApplicationLogRepository ApplicationLogsRepository) : ControllerBase
    {
        private readonly IApplicationLogRepository _ApplicationLogsRepository = ApplicationLogsRepository;

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
        [HttpGet("DB")]
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
        [HttpGet("db/categories")]
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
        [HttpGet("db/events")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<List<EventId>>> GetDatabaseLogEvents()
        {
            return await _ApplicationLogsRepository.GetLogEventsAsync();
        }
    }
}
