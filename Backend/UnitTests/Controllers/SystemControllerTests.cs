namespace UnitTests.Controllers
{
    public class SystemControllerTests
    {
        private readonly Mock<IApplicationLogRepository> _mockRepo = new();
        private readonly Mock<ISystemControllerService> _mockService = new();
        private readonly SystemController _controller;

        public SystemControllerTests()
        {
            _controller = new SystemController(_mockRepo.Object, _mockService.Object);
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
            var actionResult = Assert.IsType<ActionResult<List<string>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedCategories = Assert.IsType<List<string>>(okResult.Value);
            Assert.Equal(categories, returnedCategories);
        }
        
        [Fact]
        public void Ping_ReturnsOk()
        {
            // Act
            var result = _controller.Ping();

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

    }
}
