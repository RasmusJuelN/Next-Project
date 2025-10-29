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

        /// <summary>
        /// Downloads a specific log file from the server's file system.
        /// </summary>
        /// <param name="filename">The name of the log file to download (without path).</param>
        /// <returns>
        /// A <see cref="FileResult"/> containing the requested log file for download.
        /// </returns>
        /// <remarks>
        /// This endpoint allows administrators to download individual log files from the server.
        /// The filename parameter should only contain the file name, not the full path.
        /// The service automatically handles the correct file path and security validation.
        /// </remarks>
        /// <response code="200">Returns the log file for download</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not authorized (not an admin)</response>
        /// <response code="404">If the specified log file does not exist</response>
        [HttpGet("logs/file/{filename}")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<FileResult> GetLogFile(string filename)
        {
            return await _SystemControllerService.GetLogFile(filename);
        }

        /// <summary>
        /// Retrieves a list of all available log file names from the server's log directory.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a list of strings representing the names
        /// of all log files available for download. Returns an empty list if no log files exist.
        /// </returns>
        /// <remarks>
        /// This endpoint provides administrators with a list of all log files that can be downloaded
        /// using the GetLogFile endpoint. Only file names are returned, not full paths.
        /// The service automatically scans the configured log directory for available files.
        /// </remarks>
        /// <response code="200">Returns the list of available log file names</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not authorized (not an admin)</response>
        [HttpGet("logs/file/list")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<List<string>>> GetLogFileNames()
        {
            return Ok(_SystemControllerService.GetLogFileNames());
        }

        /// <summary>
        /// Retrieves the current system configuration settings.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a <see cref="SettingsFetchResponse"/> 
        /// with all current system configuration values.
        /// </returns>
        /// <remarks>
        /// This endpoint allows administrators to view the complete current configuration
        /// of the application. The response includes all configuration sections.
        /// </remarks>
        /// <response code="200">Returns the current system settings</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not authorized (not an admin)</response>
        [HttpGet("settings")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<SettingsFetchResponse>> GetSettings()
        {
            return Ok(await _SystemControllerService.GetSettings());
        }

        /// <summary>
        /// Retrieves the configuration schema definition that describes all available system settings.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a <see cref="SettingsSchema"/> object
        /// that defines the structure, data types, requirements, and descriptions for all
        /// configuration settings across all sections.
        /// </returns>
        /// <remarks>
        /// This endpoint provides the complete schema definition for the application's configuration.
        /// The schema includes metadata for each setting such as:
        /// - Data type (string, integer, boolean, etc.)
        /// - Whether the setting is required or optional
        /// - Description of what the setting controls
        /// - Default values where applicable
        /// - Validation rules and constraints
        /// 
        /// This is particularly useful for building dynamic configuration UIs or validation logic.
        /// The schema is dynamically generated using reflection to ensure it stays synchronized
        /// with the actual settings model definitions.
        /// </remarks>
        /// <response code="200">Returns the complete settings schema definition</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not authorized (not an admin)</response>
        [HttpGet("settings/schema")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<SettingsSchema>> GetSettingsSchema()
        {
            return Ok(await _SystemControllerService.GetSettingsSchema());
        }

        /// <summary>
        /// Retrieves the default configuration values for all system settings.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a <see cref="DefaultSettings"/> object
        /// with the default values for all configuration sections including Database, JWT,
        /// LDAP, Logging, and System settings.
        /// </returns>
        /// <remarks>
        /// This endpoint provides administrators with the factory default configuration values
        /// that would be used when creating a new installation or resetting configuration.
        /// These defaults can be used for:
        /// - Comparing current settings against defaults
        /// - Resetting individual settings to their default values
        /// - Understanding what values are considered "standard" for each setting
        /// - Configuration validation and troubleshooting
        /// 
        /// Note that these are the programmatic defaults, not necessarily the values
        /// that were used during initial application setup.
        /// </remarks>
        /// <response code="200">Returns the default system settings</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not authorized (not an admin)</response>
        [HttpGet("settings/default")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<ActionResult<DefaultSettings>> GetDefaultSettings()
        {
            return Ok(new DefaultSettings());
        }

        /// <summary>
        /// Updates system configuration settings with the provided values.
        /// </summary>
        /// <param name="settings">
        /// An <see cref="UpdateSettingsRequest"/> object containing the configuration values to update.
        /// Only non-null properties will be updated; null properties will preserve existing values.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the success or failure of the update operation.
        /// Returns HTTP 200 (OK) on successful update, or HTTP 400 (Bad Request) if the update fails.
        /// </returns>
        /// <remarks>
        /// This endpoint allows administrators to update system configuration settings.
        /// The operation performs a partial update - only the settings provided in the request
        /// will be modified, while omitted settings will retain their current values.
        /// 
        /// The update process:
        /// 1. Merges the provided settings with current configuration
        /// 2. Validates the resulting configuration
        /// 3. Writes the updated configuration to the config.json file
        /// 4. Triggers a configuration reload if supported
        /// 
        /// Use this endpoint when you need to update multiple settings or complete sections
        /// of the configuration. For smaller changes, consider using the PATCH endpoint instead.
        /// 
        /// CAUTION: Incorrect configuration values may cause application instability.
        /// Ensure all values are properly validated before submission.
        /// </remarks>
        /// <response code="200">Settings were successfully updated</response>
        /// <response code="400">The update operation failed due to invalid data or system error</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not authorized (not an admin)</response>
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

        /// <summary>
        /// Applies partial updates to system configuration settings using HTTP PATCH semantics.
        /// </summary>
        /// <param name="settings">
        /// A <see cref="PatchSettingsRequest"/> object containing the specific configuration 
        /// values to modify. Only non-null properties will be updated; null properties 
        /// will preserve existing values.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the success or failure of the patch operation.
        /// Returns HTTP 200 (OK) on successful update, or HTTP 400 (Bad Request) if the patch fails.
        /// </returns>
        /// <remarks>
        /// This endpoint allows administrators to perform fine-grained updates to system configuration.
        /// It follows HTTP PATCH semantics, meaning only the explicitly provided fields will be modified
        /// while all other configuration values remain unchanged.
        /// 
        /// The patch operation:
        /// 1. Applies only the non-null values from the request to the current configuration
        /// 2. Validates the resulting configuration for consistency
        /// 3. Persists the changes to the config.json file
        /// 4. Triggers configuration reload if supported by the system
        /// 
        /// This endpoint is ideal for:
        /// - Making small, targeted configuration changes
        /// - Updating individual settings without affecting others
        /// - Incremental configuration adjustments
        /// - Automated configuration management scenarios
        /// 
        /// The difference between PATCH and PUT is that PATCH only modifies specified fields,
        /// while PUT replaces entire sections with the provided values.
        /// 
        /// CAUTION: Some configuration changes may require application restart to take effect.
        /// </remarks>
        /// <response code="200">Settings were successfully patched</response>
        /// <response code="400">The patch operation failed due to invalid data or system error</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not authorized (not an admin)</response>
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
