
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
