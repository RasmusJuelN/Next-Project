using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API.Enums;
using API.Exceptions;
using API.DTO.LDAP;
using API.Services;
using API.Utils;
using Database.Enums;
using Database.Interfaces;
using Database.Models;
using Logging.LogEvents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Settings.Models;
using API.DTO.Responses.Auth;
using API.DTO.Requests.Auth;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly LdapService _ldapService;
        private readonly JWTSettings _JWTSettings;
        private readonly IGenericRepository<RevokedRefreshTokenModel> _revokedRefreshTokenRepository;
        private readonly IGenericRepository<UserModel> _userRepository;
        private readonly ILogger _logger;

        public AuthController(
            JwtService jwtService,
            LdapService ldapService,
            IConfiguration configuration,
            IGenericRepository<RevokedRefreshTokenModel> revokedRefreshTokenRepository,
            IGenericRepository<UserModel> userRepository,
            ILoggerFactory loggerFactory)
        {
            _jwtService = jwtService;
            _ldapService = ldapService;
            _revokedRefreshTokenRepository = revokedRefreshTokenRepository;
            _userRepository = userRepository;
            _JWTSettings = ConfigurationBinderService.Bind<JWTSettings>(configuration);
            _logger = loggerFactory.CreateLogger(GetType());
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromForm] UserLogin userLogin)
        {
            try
            {
                _ldapService.Authenticate(userLogin.Username, userLogin.Password);
            }
            catch (LDAPException.ConnectionError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (LDAPException.InvalidCredentials)
            {
                return Unauthorized();
            }

            if (_ldapService.connection.Bound)
            {
                ObjectGuidAndMemberOf ldapUser = _ldapService.SearchUser<ObjectGuidAndMemberOf>(userLogin.Username);

                if (ldapUser is null)
                {
                    _ldapService.Dispose();
                    _logger.LogWarning(UserLogEvents.UserLogIn, "User {username} successfully logged in, yet the user query returned nothing.", userLogin.Username);
                    return Unauthorized();
                }

                Guid userGuid = new(ldapUser.ObjectGUID.ByteValue);

                string userRole = _JWTSettings.Roles.FirstOrDefault(x => ldapUser.MemberOf.StringValue.Contains(x.Key)).Value;
                
                if (userRole.IsNullOrEmpty())
                {
                    _ldapService.Dispose();
                    _logger.LogWarning(UserLogEvents.UserLogIn, "Could not determine the role for user {username}", userLogin.Username);
                    return Unauthorized();
                }

                UserModel? user = await _userRepository.GetSingleAsync(u => u.Id == userGuid);

                UserPermissions permissions;
                if (user is not null)
                {
                    permissions = user.Permissions;
                }
                else
                {
                    permissions = (UserPermissions)Enum.Parse(typeof(UserPermissions), userRole, true);
                }

                JWTUser jWTUser = new()
                {
                    Guid = userGuid,
                    Username = userLogin.Username,
                    Name = ldapUser.Name.StringValue,
                    Role = userRole,
                    Permissions = (int)permissions
                };

                _ldapService.Dispose();

                List<Claim> accessTokenClaims = _jwtService.GetAccessTokenClaims(jWTUser);

                List<Claim> refreshTokenClaims = _jwtService.GetRefreshTokenClaims(jWTUser.Guid.ToString());

                AuthenticationResponse response = new()
                {
                    AuthToken = _jwtService.GenerateAccessToken(accessTokenClaims),
                    RefreshToken = _jwtService.GenerateRefreshToken(refreshTokenClaims)
                };

                _logger.LogInformation(UserLogEvents.UserLogIn, "Successfull login from {username}.", userLogin.Username);

                return Ok(response);
            }
            else return Unauthorized();
        }
        
        [Authorize(AuthenticationSchemes = "RefreshToken")]
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh([FromBody] string expiredToken)
        {
            if (Request.Headers.Authorization.IsNullOrEmpty()) return Unauthorized();

            string token = Request.Headers.Authorization!.ToString().Split(' ').Last();

            if (!_jwtService.TokenIsValid(token, _jwtService.GetRefreshTokenValidationParameters())) return Unauthorized();
            
            ClaimsPrincipal principal = _jwtService.GetPrincipalFromExpiredToken(expiredToken);

            if (User.FindFirstValue(JwtRegisteredClaimNames.Sub) != principal.FindFirstValue(JwtRegisteredClaimNames.Sub)) return Unauthorized();

            byte[] result = Crypto.ToSha256(token);
            IEnumerable<RevokedRefreshTokenModel> tokens = await _revokedRefreshTokenRepository.GetAsync(q => q.Token == result);
            if (principal is null || tokens.Any()) return Unauthorized();

            List<Claim> refreshTokenClaims = _jwtService.GetRefreshTokenClaims(principal.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

            AuthenticationResponse response = new()
            {
                AuthToken = _jwtService.GenerateAccessToken(principal.Claims),
                RefreshToken = _jwtService.GenerateRefreshToken(refreshTokenClaims)
            };

            return Ok(response);
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

            if (!_jwtService.TokenIsValid(token, _jwtService.GetRefreshTokenValidationParameters())) return Unauthorized();
            
            byte[] result = Crypto.ToSha256(token);
            
            RevokedRefreshTokenModel revokedRefreshToken = new()
            {
                Token = result,
                RevokedAt = DateTime.UtcNow
            };
            await _revokedRefreshTokenRepository.AddAsync(revokedRefreshToken);

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
                Role = User.FindFirstValue(JWTClaims.role) ?? "N/A",
                Permissions = Convert.ToInt32(User.FindFirstValue(JWTClaims.permissions) ?? "0")
            });
        }
    }

}
