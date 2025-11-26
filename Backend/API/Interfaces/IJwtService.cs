using API.DTO.Responses.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken(IEnumerable<Claim> claims);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string expiredToken);
        bool TokenIsValid(string token, TokenValidationParameters tokenValidationParameters);
        List<Claim> GetAccessTokenClaims(JWTUser user);
        List<Claim> GetRefreshTokenClaims(string userId);
        JwtSecurityTokenHandler GetTokenHandler();
        TokenValidationParameters GetAccessTokenValidationParameters();
        TokenValidationParameters GetRefreshTokenValidationParameters();

        JWTUser DecodeAccessToken(string accessToken);
    }
}
