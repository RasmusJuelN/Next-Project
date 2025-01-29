using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Enums;
using API.Exceptions;
using API.Models.LDAP;
using API.Models.Requests;
using API.Models.Responses;
using API.Services;
using Database.Enums;
using Database.Models;
using Database.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Settings.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(JWT jwt, LDAP ldap, IConfiguration configuration, IGenericRepository<RevokedRefreshTokenModel> genericRepository) : ControllerBase
    {
        private readonly JWT _JWT = jwt;
        private readonly LDAP _LDAP = ldap;
        private readonly JWTSettings _JWTSettings = new SettingsBinder(configuration).Bind<JWTSettings>();
        private readonly IGenericRepository<RevokedRefreshTokenModel> _genericRepository = genericRepository;

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(Auth), StatusCodes.Status200OK)]
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
                ObjectGuidAndMemberOf ldapUser = _LDAP.SearchUser<ObjectGuidAndMemberOf>(userLogin.Username);

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
        [ProducesResponseType(typeof(Auth), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh([FromBody] string expiredToken)
        {
            if (Request.Headers.Authorization.IsNullOrEmpty()) return Unauthorized();

            string token = Request.Headers.Authorization!.ToString().Split(' ').Last();

            if (!_JWT.TokenIsValid(token, _JWT.GetRefreshTokenValidationParameters())) return Unauthorized();
            
            ClaimsPrincipal principal = _JWT.GetPrincipalFromExpiredToken(expiredToken);

            // TODO: Also check if we've revoked the token in the database
            byte[] result = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            string stringToken = Convert.ToBase64String(result);
            IEnumerable<RevokedRefreshTokenModel> tokens = await _genericRepository.GetAsync(q => q.Token == result);
            if (principal is null || tokens.Any()) return Unauthorized();

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
        public async Task<IActionResult> Logout()
        {
            if (Request.Headers.Authorization.IsNullOrEmpty()) return Unauthorized();

            string token = Request.Headers.Authorization!.ToString().Split(' ').Last();

            if (!_JWT.TokenIsValid(token, _JWT.GetRefreshTokenValidationParameters())) return Unauthorized();
            
            byte[] result = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            // TODO: Log the token in the database so we can revoke it
            RevokedRefreshTokenModel revokedRefreshToken = new()
            {
                Token = result,
                RevokedAt = DateTime.UtcNow
            };
            await _genericRepository.AddAsync(revokedRefreshToken);

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
