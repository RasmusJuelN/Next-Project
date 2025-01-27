using System.IdentityModel.Tokens.Jwt;
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
    [Authorize(AuthenticationSchemes = "AccessToken")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(JWT JWT, LDAP LDAP, IConfiguration configuration) : ControllerBase
    {
        private readonly JWTSettings _JWTSettings = new SettingsBinder(configuration).Bind<JWTSettings>();

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(AuthenticationToken), StatusCodes.Status200OK)]
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
                    Name = ldapUser.Name.StringValue,
                    Role = userRole,
                    Permissions = permissions
                };

                LDAP.Dispose();

                AuthenticationToken authenticationToken = new()
                {
                    Token = JWT.GenerateToken(jWTUser),
                    TokenType = "bearer"
                };
                
                RefreshToken refreshToken = new()
                {
                    Token = JWT.GenerateRefreshToken(jWTUser),
                    TokenType = "bearer"
                };

                return Ok(new Login{
                    AuthToken = authenticationToken,
                    RefreshToken = refreshToken
                });
            }
            else return Unauthorized();
        }
        
        [Authorize(AuthenticationSchemes = "RefreshToken")]
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthenticationToken), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Refresh([FromBody] string token)
        {
            throw new NotImplementedException();
        }

        [HttpGet("WhoAmI")]
        [ProducesResponseType(typeof(JWTUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult WhoAmI()
        {
            if (Request.Headers.Authorization.IsNullOrEmpty()) return Forbid();

            return Ok(new JWTUser{
                Guid = new Guid(User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? "N/A"),
                Username = User.FindFirstValue(JwtRegisteredClaimNames.UniqueName) ?? "N/A",
                Name = User.FindFirstValue(JwtRegisteredClaimNames.Name) ?? "N/A",
                Role = User.FindFirstValue(JWTClaims.role)  ?? "N/A",
                Permissions = Convert.ToInt32(User.FindFirstValue(JWTClaims.permissions) ?? "0")
            });
        }
    }

}
