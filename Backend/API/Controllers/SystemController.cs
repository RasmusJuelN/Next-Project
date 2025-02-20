using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        [HttpHead("ping")]
        public IActionResult Ping()
        {
            return Ok();
        }
    }
}
