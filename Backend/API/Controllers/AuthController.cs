using System.Security.Claims;
using API.Enums;
using API.Exceptions;
using API.Models.LDAP;
using API.Models.Requests;
using API.Models.Responses;
using API.Services;
using Database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Settings.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(JWT JWT, LDAP LDAP, IConfiguration configuration) : ControllerBase
    {
        private readonly JWTSettings _JWTSettings = new SettingsBinder(configuration).Bind<JWTSettings>();

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
                ObjectGuidAndMemberOf ldapUser = LDAP.SearchUser<ObjectGuidAndMemberOf>();

                // TODO: This shuld be logged
                if (ldapUser is null) return Unauthorized();

                Guid userGuid = new(ldapUser.ObjectGUID.ByteValue);

                string userRole = _JWTSettings.Roles.FirstOrDefault(x => ldapUser.MemberOf.StringValue.Contains(x.Key)).Value;
                
                // TODO: Also log this
                if (userRole.IsNullOrEmpty()) return Unauthorized();

                // TODO: Grab permissions from the database if they exist, otherwise default to the preset that matches the role
                int permissions = (int)UserPermissions.Student;

                JWTUser jWTUser = new()
                {
                    Guid = userGuid,
                    Username = userLogin.Username,
                    Role = userRole,
                    Permissions = permissions
                };

                LDAP.Dispose();

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
