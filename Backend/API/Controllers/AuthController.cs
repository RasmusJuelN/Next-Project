using System.Security.Claims;
using API.Enums;
using API.Exceptions;
using API.Models.Requests;
using API.Models.Responses;
using API.Services;
using Database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(JWT JWT, LDAP LDAP) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Login([FromForm] UserLogin userLogin)
        {
            try
            {
                LDAP.Authenticate(userLogin.Username, userLogin.Password);
            }
            catch (LDAPException.ConnectionError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (LDAPException.InvalidCredentials)
            {
                return Unauthorized();
            }

            if (LDAP.connection.Bound)
            {
                JWTUser jWTUser = new()
                {
                    Guid = LDAP.GetObjectGuid(),
                    Username = userLogin.Username,
                    Role = UserRoles.Student.ToString(),
                    Permissions = (int)UserPermissions.Student
                };

                return Ok(new Token{
                    AccessToken = JWT.GenerateToken(jWTUser),
                    TokenType = "bearer"
                });
            }
            else return Unauthorized();
        }
        
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Refresh([FromBody] string token)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        [HttpGet("WhoAmI")]
        [ProducesResponseType(typeof(JWTUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult WhoAmI()
        {
            if (Request.Headers.Authorization.IsNullOrEmpty()) return Forbid();

            return Ok(new JWTUser{
                Guid = new Guid(User.FindFirstValue(JWTClaims.id) ?? "N/A"),
                Username = User.FindFirstValue(JWTClaims.username) ?? "N/A",
                Role = User.FindFirstValue(JWTClaims.role)  ?? "N/A",
                Permissions = Convert.ToInt32(User.FindFirstValue(JWTClaims.permissions) ?? "0")
            });
        }
    }

}
