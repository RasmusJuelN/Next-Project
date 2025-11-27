using API.DTO.User;

namespace UnitTests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthenticationBridge> _mockAuthBridge = new();
        private readonly Mock<IJwtService> _mockJwtService = new();
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<ILoggerFactory> _mockLoggerFactory = new();
        private readonly IConfiguration _configuration;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _configuration = new ConfigurationBuilder().Build();
             

            _controller = new AuthController(
                _mockJwtService.Object,
                _mockAuthBridge.Object,
                _configuration,
                _mockUnitOfWork.Object,
                loggerFactory
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            // Override private _JWTSettings using reflection
            var jwtSettings = new JWTSettings
            {
                Roles = new Dictionary<string, string>
                {
                    { "Admin", "Admin" },
                    { "User", "User" }
                }
            };

            typeof(AuthController)
                .GetField("_JWTSettings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_controller, jwtSettings);

        }
        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            // Arrange
            var login = new UserLogin { Username = "user", Password = "pass" };

            var ldapUser = new BasicUserInfoWithUserID
            {
                UserId = Guid.NewGuid().ToString(),
                Name = "Test User",
                Username = "user",
                MemberOf = ["Admin"],
            };

            _mockAuthBridge.Setup(a => a.Authenticate(login.Username, login.Password)).Verifiable();
            _mockAuthBridge.Setup(a => a.IsConnected()).Returns(true);
            _mockAuthBridge.Setup(a => a.SearchUser<BasicUserInfoWithUserID>(login.Username)).Returns(ldapUser);

            // Mock JWT service
            _mockJwtService.Setup(s => s.GetAccessTokenClaims(It.IsAny<JWTUser>())).Returns(new List<Claim>());
            _mockJwtService.Setup(s => s.GetRefreshTokenClaims(It.IsAny<string>())).Returns(new List<Claim>());
            _mockJwtService.Setup(s => s.GenerateAccessToken(It.IsAny<List<Claim>>())).Returns("access-token");
            _mockJwtService.Setup(s => s.GenerateRefreshToken(It.IsAny<List<Claim>>())).Returns("refresh-token");

            // Mock unit of work user repository
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(r => r.GetUserAsync(It.IsAny<Guid>()))
                        .ReturnsAsync((FullUser)null); // simulate user not found
            _mockUnitOfWork.Setup(u => u.User).Returns(mockUserRepo.Object);

            // Inject JWTSettings via reflection
            var jwtSettings = new JWTSettings
            {
                Roles = new Dictionary<string, string>
                {
                    { "Admin", "Admin" },
                    { "User", "User" }
                }
            };
            typeof(AuthController)
                .GetField("_JWTSettings", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_controller, jwtSettings);

            // Act
            var result = await _controller.Login(login);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthenticationResponse>(okResult.Value);
            Assert.Equal("access-token", response.AuthToken);
            Assert.Equal("refresh-token", response.RefreshToken);
        }


        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var login = new UserLogin { Username = "user", Password = "wrong" };
            _mockAuthBridge.Setup(a => a.Authenticate(login.Username, login.Password))
                .Throws(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _controller.Login(login);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials", unauthorized.Value);
        }
       
        [Fact]
        public async Task Login_LdapConnectionError_Throws()
        {
            var login = new UserLogin { Username = "user", Password = "pass" };

            _mockAuthBridge.Setup(a => a.Authenticate(login.Username, login.Password))
                .Throws(new LdapException("Connection failed", LdapException.ConnectError, null));

            await Assert.ThrowsAsync<LdapException>(() => _controller.Login(login));
        }

        [Fact]
        public async Task Login_UserRoleCannotBeDetermined_ReturnsUnauthorized()
        {
            // Arrange
            var login = new UserLogin { Username = "user", Password = "pass" };
            var ldapUser = new BasicUserInfoWithUserID
            {
                UserId = Guid.NewGuid().ToString(),
                Name = "Test User",
                Username = "testuser",
                MemberOf = ["SomeRandomGroupThatDoesNotMatchAnyRole"]

            };


            _mockAuthBridge.Setup(a => a.Authenticate(login.Username, login.Password));
            _mockAuthBridge.Setup(a => a.IsConnected()).Returns(true);
            _mockAuthBridge.Setup(a => a.SearchUser<BasicUserInfoWithUserID>(login.Username))
                           .Returns(ldapUser);

            // Act
            var result = await _controller.Login(login);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
