using API.DTO.Requests.User;
using API.DTO.Responses.User;
using API.Exceptions;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserService userService) : ControllerBase
    {
        private readonly UserService _userService = userService;

        [HttpGet]
        public ActionResult<UserQueryPaginationResult> UserPaginationQuery([FromQuery] UserQueryPagination request)
        {
            try
            {
                return Ok(_userService.QueryLDAPUsersWithPagination(request));
            }
            catch (HttpResponseException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }
    }
}
