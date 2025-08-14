using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API.Exceptions;
using API.DTO.LDAP;
using API.Services;
using API.Utils;
using Database.Enums;
using Logging.LogEvents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Settings.Models;
using API.DTO.Responses.Auth;
using API.DTO.Requests.Auth;
using Microsoft.Net.Http.Headers;
using API.Interfaces;
using Database.DTO.User;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly IAuthenticationBridge _authenticationBridge;
        private readonly JWTSettings _JWTSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public AuthController(
            JwtService jwtService,
            IAuthenticationBridge ldapService,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            ILoggerFactory loggerFactory)
        {
            _jwtService = jwtService;
            _authenticationBridge = ldapService;
            _JWTSettings = ConfigurationBinderService.Bind<JWTSettings>(configuration);
            _unitOfWork = unitOfWork;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromForm] UserLogin userLogin)
        {
            try
            {
                _authenticationBridge.Authenticate(userLogin.Username, userLogin.Password);
            }
            catch (LDAPException.ConnectionError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (LDAPException.InvalidCredentials)
            {
                return Unauthorized();
            }

            if (_authenticationBridge.IsConnected())
            {
                BasicUserInfoWithObjectGuid? ldapUser = _authenticationBridge.SearchUser<BasicUserInfoWithObjectGuid>(userLogin.Username);

                if (ldapUser is null)
                {
                    _authenticationBridge.Dispose();
                    _logger.LogWarning(UserLogEvents.UserLogIn, "User {username} successfully logged in, yet the user query returned nothing.", userLogin.Username);
                    return Unauthorized();
                }

                Guid userGuid = new(ldapUser.ObjectGUID.ByteValue);

                string userRole;
                try
                {
                    // Converts ldap role to an internal role
                    userRole = _JWTSettings.Roles.First(x => ldapUser.MemberOf.StringValue.Contains(x.Value, StringComparison.CurrentCultureIgnoreCase)).Key;
                }
                catch (Exception e)
                {
                    _authenticationBridge.Dispose();
                    _logger.LogWarning(UserLogEvents.UserLogIn, e, "User {username} successfully logged in, yet the user role could not be determined. {Message}", userLogin.Username, e.Message);
                    return Unauthorized();
                }
                
                FullUser? user = await _unitOfWork.User.GetUserAsync(userGuid);

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

                _authenticationBridge.Dispose();

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
        
        [HttpPost("refresh")]
        [Authorize(AuthenticationSchemes = "RefreshToken")]
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

            byte[] hashedToken = Crypto.ToSha256(token);
            if (principal is null || await _unitOfWork.TrackedRefreshToken.IsTokenRevoked(hashedToken)) return Unauthorized();

            List<Claim> refreshTokenClaims = _jwtService.GetRefreshTokenClaims(principal.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

            AuthenticationResponse response = new()
            {
                AuthToken = _jwtService.GenerateAccessToken(principal.Claims),
                RefreshToken = _jwtService.GenerateRefreshToken(refreshTokenClaims)
            };

            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = "RefreshToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout()
        {
            if (Request.Headers.Authorization.IsNullOrEmpty()) return Unauthorized();

            string token = Request.Headers.Authorization!.ToString().Split(' ').Last();

            if (!_jwtService.TokenIsValid(token, _jwtService.GetRefreshTokenValidationParameters())) return Unauthorized();
            
            byte[] encryptedToken = Crypto.ToSha256(token);

            await _unitOfWork.TrackedRefreshToken.RevokeToken(encryptedToken);
            await _unitOfWork.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("WhoAmI")]
        [Authorize(AuthenticationSchemes = "AccessToken")]
        [ProducesResponseType(typeof(JWTUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult WhoAmI()
        {
            if (Request.Headers.Authorization.IsNullOrEmpty()) return Forbid();
            var token = Request.Headers[HeaderNames.Authorization]
                .ToString()
                .Replace("Bearer", "")
                .Trim();

            return Ok(_jwtService.DecodeAccessToken(token));
        }
    }

}