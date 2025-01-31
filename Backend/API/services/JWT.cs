using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Settings.Models;

namespace API.Services;

public class JwtService(IConfiguration configuration)
{
    private readonly JWTSettings _JWTSettings = new SettingsBinder(configuration).Bind<JWTSettings>();
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
            ClockSkew = TimeSpan.Zero
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
            ClockSkew = TimeSpan.Zero
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
}
