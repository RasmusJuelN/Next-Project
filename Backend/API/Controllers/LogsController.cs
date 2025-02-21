using Database.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController(IGenericRepository<ApplicationLogsModel> ApplicationLogsRepository) : ControllerBase
    {
        private readonly IGenericRepository<ApplicationLogsModel> _ApplicationLogsRepository = ApplicationLogsRepository;

        [HttpGet("DB")]
        public async Task<IActionResult> GetDatabaseLogs()
        {
            throw new NotImplementedException();
        }

        [HttpGet("DBCategories")]
        public async Task<IActionResult> GetDatabaseLogCategories()
        {
            List<string> categories = await _ApplicationLogsRepository.GetAsQueryable()
                .Select(q => q.Category).Distinct().ToListAsync();
            
            return Ok(categories);
        }
    }
}
