using API.DTO.Responses.Auth;
using API.Services;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace UnitTests.Services
{
    public class JwtServiceTests
    {
       private readonly JwtService _jwtService;
        private readonly string _accessSecret = "SuperSecretAccessKey1234567890123456";
        private readonly string _refreshSecret = "SuperSecretRefreshKey12345678901234";
        private readonly string _issuer = "TestIssuer";
        private readonly string _audience = "TestAudience";

        public JwtServiceTests()
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                { "JWT:AccessTokenSecret", _accessSecret },
                { "JWT:RefreshTokenSecret", _refreshSecret },
                { "JWT:Issuer", _issuer },
                { "JWT:Audience", _audience },
                { "JWT:TokenTTLMinutes", "60" },
                { "JWT:RenewTokenTTLDays", "6" }
            };


             IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

            _jwtService = new JwtService(configuration);
        }


        [Fact]
        public void GenerateAccessToken_ShouldReturnToken()
        {
            var claims = new List<Claim> { new Claim("sub", Guid.NewGuid().ToString()) };
            var token = _jwtService.GenerateAccessToken(claims);
            Assert.False(string.IsNullOrEmpty(token));
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturnToken()
        {
            var claims = new List<Claim> { new Claim("sub", Guid.NewGuid().ToString()) };
            var token = _jwtService.GenerateRefreshToken(claims);
            Assert.False(string.IsNullOrEmpty(token));
        }

        [Fact]
        public void TokenIsValid_ShouldReturnTrueForValidToken()
        {
            var claims = new List<Claim> { new Claim("sub", "123") };
            var token = _jwtService.GenerateAccessToken(claims);

            var parameters = JwtService.GetAccessTokenValidationParameters(_accessSecret, _issuer, _audience);
            bool isValid = _jwtService.TokenIsValid(token, parameters);

            Assert.True(isValid);
        }

        [Fact]
        public void TokenIsValid_ShouldReturnFalseForInvalidToken()
        {
            string tamperedToken = "invalid.token.value";
            var parameters = JwtService.GetAccessTokenValidationParameters(_accessSecret, _issuer, _audience);
            bool isValid = _jwtService.TokenIsValid(tamperedToken, parameters);
            Assert.False(isValid);
        }

        [Fact]
        public void DecodeAccessToken_ShouldReturnCorrectClaims()
        {
            var user = new JWTUser
            {
                Guid = Guid.NewGuid(),
                Username = "testuser",
                Name = "Test User",
                Role = "Admin",
                Permissions = 7
            };

            var claims = _jwtService.GetAccessTokenClaims(user);
            var token = _jwtService.GenerateAccessToken(claims);
            var decodedUser = _jwtService.DecodeAccessToken(token);

            //var token = _jwtService.GenerateAccessToken(_jwtService.GetAccessTokenClaims(user));
            //var decodedUser = _jwtService.DecodeAccessToken(token);

            Assert.Equal(user.Guid, decodedUser.Guid);
            Assert.Equal(user.Username, decodedUser.Username);
            Assert.Equal(user.Name, decodedUser.Name);
            Assert.Equal(user.Role, decodedUser.Role);
            Assert.Equal(user.Permissions, decodedUser.Permissions);
        }

        [Fact]
        public void GetPrincipalFromExpiredToken_ShouldReturnClaims()
        {
            var claims = new List<Claim> { new Claim("sub", "123") };
            var token = _jwtService.GenerateAccessToken(claims);

            // Use proper validation parameters with audience/issuer
            var principal = _jwtService.GetPrincipalFromExpiredToken(token);

            Assert.NotNull(principal);
            Assert.Equal("123", principal.FindFirst("sub")?.Value);
        }

    }
}
