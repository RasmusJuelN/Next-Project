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
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(JWT _JWT, LDAP _LDAP, IConfiguration configuration) : ControllerBase
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
                _LDAP.Authenticate(userLogin.Username, userLogin.Password);
            }
            catch (LDAPException.ConnectionError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (LDAPException.InvalidCredentials)
            {
                return Unauthorized();
            }

            if (_LDAP.connection.Bound)
            {
                ObjectGuidAndMemberOf ldapUser = _LDAP.SearchUser<ObjectGuidAndMemberOf>();

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

                _LDAP.Dispose();

                Claim[] accessTokenClaims =
                [
                    new Claim(JwtRegisteredClaimNames.Sub, jWTUser.Guid.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, jWTUser.Username),
                    new Claim(JwtRegisteredClaimNames.Name, jWTUser.Name),
                    new Claim(JWTClaims.role, jWTUser.Role),
                    new Claim(JWTClaims.permissions, jWTUser.Permissions.ToString()),
                    new Claim(JwtRegisteredClaimNames.Iss, _JWTSettings.Issuer),
                    new Claim(JwtRegisteredClaimNames.Aud, _JWTSettings.Audience),
                ];

                Claim[] refreshTokenClaims =
                [
                    new Claim(JwtRegisteredClaimNames.Sub, jWTUser.Guid.ToString()),
                ];

                AuthenticationToken authenticationToken = new()
                {
                    Token = _JWT.GenerateAccessToken(accessTokenClaims),
                    TokenType = "bearer"
                };
                
                RefreshToken refreshToken = new()
                {
                    Token = _JWT.GenerateRefreshToken(refreshTokenClaims),
                    TokenType = "bearer"
                };

                return Ok(new Auth{
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
        public IActionResult Refresh([FromBody] string expiredToken)
        {
            ClaimsPrincipal principal = _JWT.GetPrincipalFromExpiredToken(expiredToken);

            // TODO: Also check if we've revoked the token in the database
            if (principal is null) return Unauthorized();

            AuthenticationToken authenticationToken = new()
            {
                Token = _JWT.GenerateAccessToken(principal.Claims),
                TokenType = "bearer"
            };

            Claim[] refreshTokenClaims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, principal.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? throw new ArgumentNullException(nameof(JwtRegisteredClaimNames.Sub))),
            ];

            RefreshToken refreshToken = new()
            {
                Token = _JWT.GenerateRefreshToken(refreshTokenClaims),
                TokenType = "bearer"
            };

            return Ok(new Auth{
                AuthToken = authenticationToken,
                RefreshToken = refreshToken
            });
        }

        [Authorize(AuthenticationSchemes = "RefreshToken")]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Logout()
        {
            if (!Request.Headers.Authorization.IsNullOrEmpty()) return Unauthorized();

            if (!_JWT.TokenIsValid(Request.Headers.Authorization, _JWT.GetRefreshTokenValidationParameters())) return Unauthorized();
            
            // TODO: Log the token in the database so we can revoke it
            
            return Ok();
        }

        [Authorize(AuthenticationSchemes = "AccessToken")]
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
