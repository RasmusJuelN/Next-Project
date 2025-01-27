using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Enums;
using API.Models.Responses;
using Microsoft.IdentityModel.Tokens;
using Settings.Models;

namespace API.Services;

public class JWT(IConfiguration configuration)
{
    private readonly JWTSettings JWTSettings = new SettingsBinder(configuration).Bind<JWTSettings>();
    public string GenerateToken(JWTUser user)
    {
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Guid.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Name, user.Name),
            new(JWTClaims.role, user.Role.ToString()),
            new(JWTClaims.permissions, user.Permissions.ToString()),
            new(JwtRegisteredClaimNames.Iss, JWTSettings.Issuer),
            new(JwtRegisteredClaimNames.Aud, JWTSettings.Audience)
        ];

        JwtSecurityToken jwtSecurityToken = new(
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(JWTSettings.TokenTTLMinutes),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(JWTSettings.AuthenticationTokenSecret)
                ),
                SecurityAlgorithms.HmacSha256
            )
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }

    public string GenerateRefreshToken(JWTUser user)
    {
        List<Claim> claims =
        [
            new Claim(JWTClaims.id, user.Guid.ToString()),
            new Claim(JwtRegisteredClaimNames.Iss, JWTSettings.Issuer),
            new Claim(JwtRegisteredClaimNames.Aud, JWTSettings.Audience)
        ];

        JwtSecurityToken jwtSecurityToken = new(
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddDays(JWTSettings.RenewTokenTTLDays),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(JWTSettings.RefreshTokenSecret)
                ),
                SecurityAlgorithms.HmacSha256
            )
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}
