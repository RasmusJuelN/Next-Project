using Database.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
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

        [HttpGet("DBCategories")]
        [Authorize(AuthenticationSchemes = "AccessToken", Policy = "AdminOnly")]
        public async Task<IActionResult> GetDatabaseLogCategories()
        {
            return Ok(await _ApplicationLogsRepository.GetLogCategoriesAsync());
        }
    }
}
