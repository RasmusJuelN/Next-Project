
namespace UnitTests.Controllers
{
    public class QuestionnaireTemplateControllerTests
    {
        private readonly Mock<IQuestionnaireTemplateService> _mockService;
        private readonly QuestionnaireTemplateController _controller;

        public QuestionnaireTemplateControllerTests()
        {
            // Mock the interface, not the concrete service
            _mockService = new Mock<IQuestionnaireTemplateService>();
            _controller = new QuestionnaireTemplateController(_mockService.Object);
        }

        #region GetTemplateById

        [Fact]
        public async Task GetQuestionnaireTemplate_ReturnsOk_WhenTemplateExists()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var template = new QuestionnaireTemplate
            {
                Id = templateId,
                Title = "Template",
                Description = "Desc",
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                IsLocked = false,
                TemplateStatus = TemplateStatus.Draft,
                Questions = new List<QuestionnaireTemplateQuestion>()
            };

            _mockService.Setup(s => s.GetTemplateById(templateId))
                        .ReturnsAsync(template);

            // Act
            var result = await _controller.GetQuestionnaireTemplate(templateId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTemplate = Assert.IsType<QuestionnaireTemplate>(okResult.Value);
            Assert.Equal(templateId, returnedTemplate.Id);
            Assert.Equal("Template", returnedTemplate.Title);
        }

        [Fact]
        public async Task GetQuestionnaireTemplate_ReturnsNotFound_WhenTemplateDoesNotExist()
        {
            // Arrange
            var templateId = Guid.NewGuid();

            _mockService.Setup(s => s.GetTemplateById(It.IsAny<Guid>()))
                        .ThrowsAsync(new SQLException.ItemNotFound());

            // Act
            var result = await _controller.GetQuestionnaireTemplate(templateId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        #endregion

        #region AddTemplate

        [Fact]
        public async Task AddQuestionnaireTemplate_ReturnsCreated_WhenSuccessful()
        {
            // Arrange
            var addRequest = new QuestionnaireTemplateAdd
            {
                Title = "New Template",
                Description = "Desc",
                Questions = new List<QuestionnaireQuestionAdd>()
            };

            var createdTemplate = new QuestionnaireTemplate
            {
                Id = Guid.NewGuid(),
                Title = addRequest.Title,
                Description = addRequest.Description,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                IsLocked = false,
                TemplateStatus = TemplateStatus.Draft,
                Questions = new List<QuestionnaireTemplateQuestion>()
            };

            _mockService.Setup(s => s.AddTemplate(It.Is<QuestionnaireTemplateAdd>(r => r.Title == addRequest.Title)))
                        .ReturnsAsync(createdTemplate);

            // Act
            var result = await _controller.AddQuestionnaireTemplate(addRequest);

            // Assert
            var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
            var returnedTemplate = Assert.IsType<QuestionnaireTemplate>(createdResult.Value);
            Assert.Equal(addRequest.Title, returnedTemplate.Title);
            Assert.Equal(addRequest.Description, returnedTemplate.Description);
            Assert.Equal(TemplateStatus.Draft, returnedTemplate.TemplateStatus);
            Assert.Empty(returnedTemplate.Questions);
        }

        [Fact]
        public async Task AddQuestionnaireTemplate_ReturnsConflict_WhenTemplateExists()
        {
            // Arrange
            var addRequest = new QuestionnaireTemplateAdd
            {
                Title = "Existing Template",
                Description = "Desc",
                Questions = new List<QuestionnaireQuestionAdd>()
            };

            _mockService.Setup(s => s.AddTemplate(It.IsAny<QuestionnaireTemplateAdd>()))
                        .ThrowsAsync(new SQLException.ItemAlreadyExists());

            // Act
            var result = await _controller.AddQuestionnaireTemplate(addRequest);

            // Assert
            Assert.IsType<ConflictResult>(result.Result);
        }

        #endregion

        #region DeleteTemplate

        [Fact]
        public async Task DeleteQuestionnaireTemplate_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteTemplate(templateId))
                        .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteQuestionnaireTemplate(templateId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteQuestionnaireTemplate_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteTemplate(It.IsAny<Guid>()))
                        .ThrowsAsync(new SQLException.ItemNotFound());

            // Act
            var result = await _controller.DeleteQuestionnaireTemplate(templateId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion


    }
}
