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
            new(JWTClaims.id, user.Guid.ToString()),
            new(JWTClaims.username, user.Username),
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
                    Encoding.UTF8.GetBytes(JWTSettings.Secret)
                ),
                SecurityAlgorithms.HmacSha256
            )
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}
