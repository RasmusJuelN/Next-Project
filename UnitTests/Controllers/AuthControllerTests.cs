using API.Controllers;
using API.DTO.LDAP;
using API.DTO.Requests.Auth;
using API.DTO.Responses.Auth;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Novell.Directory.Ldap;
using Settings.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthenticationBridge> _mockAuthBridge = new();
        private readonly Mock<IJwtService> _mockJwtService = new();
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<ILoggerFactory> _mockLoggerFactory = new();
        private readonly Mock<ILogger> _mockLogger = new();
        private readonly IConfiguration _configuration;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { { "JWTS:Roles:Admin", "Admin" } })
                .Build();

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
            
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            // Arrange
            var login = new UserLogin { Username = "user", Password = "pass" };

            var ldapUser = new BasicUserInfoWithObjectGuid
            {
                ObjectGUID = new LdapAttribute("objectGUID", Guid.NewGuid().ToByteArray()),
                Name = new LdapAttribute("name", "Test User"),
                Username = new LdapAttribute("sAMAccountName", "user"),
                MemberOf = new LdapAttribute("memberOf", "Admin")
            };

            //_mockAuthBridge.Setup(a => a.Authenticate(login.Username, login.Password));
            _mockAuthBridge.Setup(a => a.Authenticate(login.Username, login.Password)).Verifiable();

            _mockAuthBridge.Setup(a => a.IsConnected()).Returns(true);
            _mockAuthBridge.Setup(a => a.SearchUser<BasicUserInfoWithObjectGuid>(login.Username)).Returns(ldapUser);

            _mockJwtService.Setup(s => s.GetAccessTokenClaims(It.IsAny<JWTUser>())).Returns(new List<Claim>());
            _mockJwtService.Setup(s => s.GetRefreshTokenClaims(It.IsAny<string>())).Returns(new List<Claim>());
            _mockJwtService.Setup(s => s.GenerateAccessToken(It.IsAny<List<Claim>>())).Returns("access-token");
            _mockJwtService.Setup(s => s.GenerateRefreshToken(It.IsAny<List<Claim>>())).Returns("refresh-token");

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
        public async Task Login_LdapConnectionError_ReturnsInternalServerError()
        {
            // Arrange
            var login = new UserLogin { Username = "user", Password = "pass" };

            // Throw a generic LdapException
            _mockAuthBridge.Setup(a => a.Authenticate(login.Username, login.Password))
                .Throws(new LdapException()); 

            // Act
            var result = await _controller.Login(login);

            // Assert
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
        }


        //[Fact]
        //public async Task Login_LdapConnectionError_ReturnsInternalServerError()
        //{
        //    // Arrange
        //    var login = new UserLogin { Username = "user", Password = "pass" };

        //    // Throw a generic LdapException
        //    _mockAuthBridge.Setup(a => a.Authenticate(login.Username, login.Password))
        //        .Throws(new Novell.Directory.Ldap.LdapException());

        //    // Act
        //    var result = await _controller.Login(login);

        //    // Assert
        //    var statusResult = Assert.IsType<ObjectResult>(result);
        //    Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
        //}

        [Fact]
        public async Task Login_UserRoleCannotBeDetermined_ReturnsUnauthorized()
        {
            // Arrange
            var login = new UserLogin { Username = "user", Password = "pass" };
            var ldapUser = new BasicUserInfoWithObjectGuid
            {
                ObjectGUID = new LdapAttribute("objectGUID", Guid.NewGuid().ToByteArray()),
                Name = new LdapAttribute("name", "Test User"),
                Username = new LdapAttribute("sAMAccountName", "testuser"),
                MemberOf = new LdapAttribute("memberOf", "Admin")
            };


            _mockAuthBridge.Setup(a => a.Authenticate(login.Username, login.Password));
            _mockAuthBridge.Setup(a => a.IsConnected()).Returns(true);
            _mockAuthBridge.Setup(a => a.SearchUser<BasicUserInfoWithObjectGuid>(login.Username))
                           .Returns(ldapUser);

            // Act
            var result = await _controller.Login(login);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
