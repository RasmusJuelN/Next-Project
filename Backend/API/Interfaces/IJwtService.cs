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
