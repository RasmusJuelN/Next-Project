using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API.DTO.User;
using API.DTO.Requests.Auth;
using API.DTO.Responses.Auth;
using API.Exceptions;
using API.Interfaces;
using API.Services;
using API.Utils;
using Database.DTO.User;
using Database.Enums;
using Logging.LogEvents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Settings.Models;

namespace API.Controllers
{
    /// <summary>
    /// Controller responsible for handling authentication and authorization operations.
    /// Provides endpoints for user login, token refresh, logout, and user information retrieval.
    /// </summary>
    /// <remarks>
    /// This controller integrates with LDAP for user authentication and uses JWT tokens for authorization.
    /// It supports the following authentication flows:
    /// - Initial authentication via LDAP credentials
    /// - Token refresh using valid refresh tokens
    /// - User logout with token revocation
    /// - Current user information retrieval
    /// 
    /// The controller uses dependency injection for:
    /// - JWT service for token operations
    /// - Authentication bridge for LDAP integration
    /// - Unit of work pattern for data access
    /// - Configuration settings for JWT parameters
    /// - Logging for security monitoring
    /// 
    /// All endpoints include comprehensive error handling and security logging.
    /// Tokens are properly validated and refresh tokens are tracked for revocation.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // TODO: Add a service to move the majority of the code out of the controller
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

        /// <summary>
        /// Authenticates a user using LDAP credentials and generates JWT tokens for authorization.
        /// </summary>
        /// <param name="userLogin">The user login credentials containing username and password</param>
        /// <returns>
        /// Returns an <see cref="AuthenticationResponse"/> with access and refresh tokens on successful authentication,
        /// or an appropriate error status code on failure.
        /// </returns>
        /// <response code="200">Returns the authentication response with JWT tokens</response>
        /// <response code="401">Returned when authentication fails due to invalid credentials or user not found</response>
        /// <response code="500">Returned when there is an LDAP connection error</response>
        /// <remarks>
        /// This endpoint performs the following operations:
        /// 1. Authenticates the user against LDAP using provided credentials
        /// 2. Retrieves user information from LDAP directory
        /// 3. Maps LDAP roles to internal application roles
        /// 4. Fetches or determines user permissions from the database
        /// 5. Generates JWT access and refresh tokens
        /// 6. Returns the authentication response with tokens
        /// 
        /// The authentication process includes proper disposal of LDAP connections and comprehensive logging
        /// for security monitoring purposes.
        /// </remarks>
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
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }

            if (_authenticationBridge.IsConnected())
            {
                BasicUserInfoWithUserID? ldapUser = _authenticationBridge.SearchUser<BasicUserInfoWithUserID>(userLogin.Username);

                if (ldapUser is null)
                {
                    _authenticationBridge.Dispose();
                    _logger.LogWarning(UserLogEvents.UserLogIn, "User {username} successfully logged in, yet the user query returned nothing.", userLogin.Username);
                    return Unauthorized();
                }

                Guid userGuid = new(ldapUser.UserId);

                string userRole;
                try
                {
                    // Converts ldap role to an internal role
                    userRole = _JWTSettings.Roles.First(x => ldapUser.MemberOf.Any(y => y.Contains(x.Value, StringComparison.OrdinalIgnoreCase))).Key;
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
                    Name = ldapUser.Name,
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

        /// <summary>
        /// Refreshes an expired access token using a valid refresh token.
        /// </summary>
        /// <param name="expiredToken">The expired access token that needs to be refreshed.</param>
        /// <returns>
        /// Returns an <see cref="AuthenticationResponse"/> containing a new access token and refresh token if successful.
        /// Returns 401 Unauthorized if:
        /// - No authorization header is provided
        /// - The refresh token is invalid or expired
        /// - The subject claims don't match between tokens
        /// - The refresh token has been revoked
        /// Returns 500 Internal Server Error for unexpected errors.
        /// </returns>
        /// <remarks>
        /// This endpoint requires authorization with the "RefreshToken" authentication scheme.
        /// The refresh token must be provided in the Authorization header as a Bearer token.
        /// The expired access token is passed in the request body and is used to validate the token refresh request.
        /// Upon successful validation, new access and refresh tokens are generated and returned.
        /// </remarks>
        [HttpPost("refresh")]
        [Authorize(AuthenticationSchemes = "RefreshToken")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            if (Request.Headers.Authorization.IsNullOrEmpty()) return Unauthorized();

            string token = Request.Headers.Authorization!.ToString().Split(' ').Last();

            if (!_jwtService.TokenIsValid(token, _jwtService.GetRefreshTokenValidationParameters())) return Unauthorized();

            ClaimsPrincipal principal = _jwtService.GetPrincipalFromExpiredToken(request.ExpiredToken);

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

        /// <summary>
        /// Logs out the user by revoking their refresh token.
        /// </summary>
        /// <returns>
        /// Returns HTTP 200 OK if the logout is successful.
        /// Returns HTTP 401 Unauthorized if the authorization header is missing or the token is invalid.
        /// Returns HTTP 500 Internal Server Error if an unexpected error occurs during the logout process.
        /// </returns>
        /// <remarks>
        /// This endpoint requires authorization with a refresh token. The refresh token is extracted from the Authorization header,
        /// validated, and then revoked by adding it to the tracked refresh tokens list. The token is hashed using SHA-256 before storage.
        /// </remarks>
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

        /// <summary>
        /// Retrieves the current authenticated user's information from the access token.
        /// </summary>
        /// <returns>
        /// Returns the user information decoded from the JWT access token if authentication is successful.
        /// Returns 401 Unauthorized if the user is not authenticated or the token is invalid.
        /// Returns 403 Forbidden if the Authorization header is missing.
        /// </returns>
        /// <response code="200">Returns the current user's information from the JWT token</response>
        /// <response code="401">User is not authenticated or token is invalid</response>
        /// <response code="403">Authorization header is missing</response>
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