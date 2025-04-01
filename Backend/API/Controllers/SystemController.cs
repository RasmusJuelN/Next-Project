using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        [HttpHead("ping")]
        [AllowAnonymous]
        public IActionResult Ping()
        {
            return Ok();
        }
    }
}
