using API.DTO.Responses.Auth;
using API.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
                { "JWTSettings:AccessTokenSecret", _accessSecret },
                { "JWTSettings:RefreshTokenSecret", _refreshSecret },
                { "JWTSettings:Issuer", _issuer },
                { "JWTSettings:Audience", _audience },
                { "JWTSettings:TokenTTLMinutes", "60" },
                { "JWTSettings:RenewTokenTTLDays", "6" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _jwtService = new JwtService(configuration);
        }



        [Fact]
        public void TokenIsValid_ShouldReturnFalseForInvalidToken()
        {
            string tamperedToken = "invalid.token.value";
            var parameters = JwtService.GetAccessTokenValidationParameters(_accessSecret, _issuer, _audience);
            bool isValid = _jwtService.TokenIsValid(tamperedToken, parameters);
            Assert.False(isValid);
        }

        
    }
}
