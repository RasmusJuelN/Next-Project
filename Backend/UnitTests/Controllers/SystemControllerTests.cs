using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Controllers
{
    public class SystemControllerTests
    {
        [Fact]
        public void Ping_ReturnsOk()
        {
            // Arrange
            var controller = new SystemController();

            // Act
            var result = controller.Ping();

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

    }
}
