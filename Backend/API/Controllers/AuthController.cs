using API.Exceptions;
using API.Models.Requests;
using API.Models.Responses;
using API.Services;
using Database.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(JWT JWT, LDAP LDAP) : ControllerBase
    {
        [HttpPost("login")]
        [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Login([FromForm] UserLogin userLogin)
        {
            try
            {
                LDAP.Authenticate(userLogin.Username, userLogin.Password);
            }
            catch (LDAPException.ConnectionErrorException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (LDAPException.InvalidCredentialsException)
            {
                return Unauthorized();
            }

            if (LDAP.connection.Bound)
            {
                JWTUser jWTUser = new()
                {
                    Guid = Guid.NewGuid(),
                    Username = userLogin.Username,
                    Role = UserRoles.Student,
                    Permissions = (int)UserPermissions.Student
                };

                return Ok(new Token{
                    AccessToken = JWT.GenerateToken(jWTUser),
                    TokenType = "bearer"
                });
            }
            else return Unauthorized();
        }
        [HttpPost("renew")]
        [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Renew([FromBody] string token)
        {
            throw new NotImplementedException();
        }
    }

}
