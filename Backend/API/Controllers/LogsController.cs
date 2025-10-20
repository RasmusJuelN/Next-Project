using System.Reflection;
using Database.Interfaces;
using Database.Models;
using Logging.LogEvents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("DB")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<IActionResult> GetDatabaseLogs()
        {
            throw new NotImplementedException();
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
        public async Task<ActionResult<EventId>> GetDatabaseLogEvents()
        {
            List<int> eventIDs = await _ApplicationLogsRepository.GetLogEventIDsAsync();

            List<LogEventsBase> eventTypes = [];

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsSubclassOf(typeof(LogEventsBase)))
                        {
                            if (Activator.CreateInstance(type) is LogEventsBase instance)
                            {
                                eventTypes.Add(instance);
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    // Ignore assemblies that cannot be loaded
                }
            }

            List<EventId> eventIds = [];

            foreach (LogEventsBase eventType in eventTypes)
            {
                FieldInfo[] fieldInfos = eventType.GetType().GetFields(BindingFlags.Public | BindingFlags.Static).Where(static field => field.FieldType == typeof(EventId)).ToArray();

                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    EventId eventId = (EventId)fieldInfo.GetValue(null)!;
                    eventIds.Add(eventId);
                }
            }

            return Ok(eventIds.Where(e => eventIDs.Contains(e.Id)).ToList());
        }
    }
}
