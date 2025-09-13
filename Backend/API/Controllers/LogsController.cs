using Database.Interfaces;
using Database.Models;
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
        public IActionResult GetDatabaseLogs()
        {
            throw new NotImplementedException();
        }

        [HttpGet("DBCategories")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<IActionResult> GetDatabaseLogCategories()
        {
            return Ok(await _ApplicationLogsRepository.GetLogCategoriesAsync());
        }
    }
}
