using API.DTO.Responses.Auth;
using API.Enums;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Settings.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace API.Services;

/// <summary>
/// Provides JSON Web Token (JWT) generation, validation, and management services for authentication and authorization.
/// This service handles both access tokens (short-lived) and refresh tokens (long-lived) with separate signing keys
/// to ensure security isolation between token types.
/// </summary>
/// <remarks>
/// The service implements a dual-token authentication strategy:
/// <list type="bullet">
/// <item><description>Access tokens: Short-lived tokens for API access with configurable expiration</description></item>
/// <item><description>Refresh tokens: Long-lived tokens for obtaining new access tokens</description></item>
/// </list>
/// All tokens use HMAC-SHA256 signing and include comprehensive validation to prevent token tampering.
/// </remarks>

public class JwtService : IJwtService
{

    private readonly JWTSettings _JWTSettings;
    public JwtService(IConfiguration configuration)
    {
        _JWTSettings = ConfigurationBinderService.Bind<JWTSettings>(configuration);
    }



    /// <summary>
    /// Generates a short-lived access token containing the specified claims.
    /// </summary>
    /// <param name="claims">The collection of claims to include in the access token.</param>
    /// <returns>A JWT access token string signed with the access token secret.</returns>
    /// <remarks>
    /// Access tokens are designed for API authentication and have a shorter lifespan
    /// as configured in the JWT settings. They use a separate signing key from refresh tokens
    /// to provide security isolation. The token includes:
    /// <list type="bullet">
    /// <item><description>User claims and roles for authorization</description></item>
    /// <item><description>Configurable expiration time (typically 15-60 minutes)</description></item>
    /// <item><description>Not-before time set to current UTC time</description></item>
    /// </list>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when claims collection is null.</exception>
    /// <exception cref="SecurityTokenException">Thrown when token generation fails.</exception>
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        JwtSecurityToken jwtSecurityToken = new(
            //added issuer and audience 
            // IMPORTANT: Token validation checks both the issuer ("iss") and audience ("aud") fields
            // against what is configured in TokenValidationParameters. 
            // If you only include these values as claims, the validator will fail:
            //   - TokenIsValid() returns false
            //   - GetPrincipalFromExpiredToken() throws SecurityTokenInvalidAudienceException
            // By passing issuer and audience to the JwtSecurityToken constructor,
            // the JWT contains the correct header/payload fields and validation passes.
            // This is why the test now succeeds.
            issuer: _JWTSettings.Issuer,       
            audience: _JWTSettings.Audience,

            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_JWTSettings.TokenTTLMinutes),
            signingCredentials: GetSigningCredentials(_JWTSettings.AccessTokenSecret)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }

    /// <summary>
    /// Generates a long-lived refresh token containing the specified claims.
    /// </summary>
    /// <param name="claims">The collection of claims to include in the refresh token.</param>
    /// <returns>A JWT refresh token string signed with the refresh token secret.</returns>
    /// <remarks>
    /// Refresh tokens are used to obtain new access tokens without requiring user re-authentication.
    /// They have a longer lifespan and use a separate signing key for enhanced security:
    /// <list type="bullet">
    /// <item><description>Extended expiration time (typically 7-30 days)</description></item>
    /// <item><description>Separate signing key from access tokens</description></item>
    /// <item><description>Should be stored securely and rotated regularly</description></item>
    /// </list>
    /// The refresh token should only be used for token renewal operations.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when claims collection is null.</exception>
    /// <exception cref="SecurityTokenException">Thrown when token generation fails.</exception>
    public string GenerateRefreshToken(IEnumerable<Claim> claims)
    {
        JwtSecurityToken jwtSecurityToken = new(
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddDays(_JWTSettings.RenewTokenTTLDays),
            signingCredentials: GetSigningCredentials(_JWTSettings.RefreshTokenSecret)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string expiredToken)
    {
        JwtSecurityTokenHandler tokenHandler = GetTokenHandler();
        
        TokenValidationParameters tokenValidationParameters = GetAccessTokenValidationParameters();
        tokenValidationParameters.ValidateLifetime = false;

        ClaimsPrincipal principal = tokenHandler.ValidateToken(expiredToken, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    public bool TokenIsValid(string token, TokenValidationParameters tokenValidationParameters)
    {
        JwtSecurityTokenHandler tokenHandler = GetTokenHandler();

        try
        {
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public List<Claim> GetAccessTokenClaims(JWTUser user)
    {
        return [
            new(JwtRegisteredClaimNames.Sub, user.Guid.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Name, user.Name),
            new(JWTClaims.role, user.Role),
            new(JWTClaims.permissions, user.Permissions.ToString()),
            new(JwtRegisteredClaimNames.Iss, _JWTSettings.Issuer),
            new(JwtRegisteredClaimNames.Aud, _JWTSettings.Audience)
        ];
    }

    public List<Claim> GetRefreshTokenClaims(string userId)
    {
        return [
            new(JwtRegisteredClaimNames.Sub, userId),
        ];
    }
    public JwtSecurityTokenHandler GetTokenHandler()
    {
        return new()
        {
            MapInboundClaims = false
        };
    }

    public TokenValidationParameters GetAccessTokenValidationParameters()
    {
        return new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateActor = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _JWTSettings.Issuer,
            ValidAudience = _JWTSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWTSettings.AccessTokenSecret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    public static TokenValidationParameters GetAccessTokenValidationParameters(string secret, string issuer, string audience)
    {
        return new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateActor = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = "role" // We're telling ASP.NET to use this claim for role-based authorization checking
        };
    }

    public TokenValidationParameters GetRefreshTokenValidationParameters()
    {
        return new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateActor = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWTSettings.RefreshTokenSecret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
    }

    public static TokenValidationParameters GetRefreshTokenValidationParameters(string secret)
    {
        return new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateActor = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    public static SigningCredentials GetSigningCredentials(string secret)
    {
        return new(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256
        );
    }

    public JWTUser DecodeAccessToken(string accessToken)
    {
        JwtSecurityTokenHandler tokenHandler = GetTokenHandler();

        JwtSecurityToken jwtSecurityToken = tokenHandler.ReadJwtToken(accessToken);

        return new JWTUser
        {
            Guid = Guid.Parse(jwtSecurityToken.Subject),
            Username = jwtSecurityToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.UniqueName).Value,
            Name = jwtSecurityToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Name).Value,
            Role = jwtSecurityToken.Claims.First(x => x.Type == JWTClaims.role).Value,
            Permissions = int.Parse(jwtSecurityToken.Claims.First(x => x.Type == JWTClaims.permissions).Value)
        };
    }
}
