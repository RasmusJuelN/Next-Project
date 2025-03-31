using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Enums;
using Microsoft.IdentityModel.Tokens;
using Settings.Models;
using API.DTO.Responses.Auth;

namespace API.Services;

public class JwtService(IConfiguration configuration)
{
    private readonly JWTSettings _JWTSettings = ConfigurationBinderService.Bind<JWTSettings>(configuration);
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        JwtSecurityToken jwtSecurityToken = new(
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_JWTSettings.TokenTTLMinutes),
            signingCredentials: GetSigningCredentials(_JWTSettings.AccessTokenSecret)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }

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

    public static JwtSecurityTokenHandler GetTokenHandler()
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
