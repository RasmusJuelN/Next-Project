using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public class SystemController : ControllerBase
    {
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
    }
}
