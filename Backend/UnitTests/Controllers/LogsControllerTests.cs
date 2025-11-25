using API.Controllers;
using Database.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Controllers
{
    public class LogsControllerTests
    {
        private readonly Mock<IApplicationLogRepository> _mockRepo = new();
        private readonly LogsController _controller;

        public LogsControllerTests()
        {
            _controller = new LogsController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetDatabaseLogCategories_ReturnsOkWithCategories()
        {
            // Arrange
            var categories = new List<string> { "Error", "Warning", "Info" };
            _mockRepo.Setup(r => r.GetLogCategoriesAsync()).ReturnsAsync(categories);

            // Act
            var result = await _controller.GetDatabaseLogCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCategories = Assert.IsAssignableFrom<IEnumerable<string>>(okResult.Value);
            Assert.Equal(categories, returnedCategories);
        }
    }
}
