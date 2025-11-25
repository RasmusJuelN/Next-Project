using API.DTO.Requests.QuestionnaireTemplate;
using API.Exceptions;
using API.Interfaces;
using API.Services;
using Database.DTO.QuestionnaireTemplate;
using Database.Enums;
using Moq;

namespace UnitTests.Services
{
    public class QuestionnaireTemplateServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly QuestionnaireTemplateService _service;

        public QuestionnaireTemplateServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _service = new QuestionnaireTemplateService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task AddTemplate_ShouldReturnAddedTemplate()
        {
            // Arrange
            var request = new QuestionnaireTemplateAdd
            {
                Title = "Template 1",
                Description = "Description",
                Questions = new List<QuestionnaireQuestionAdd>()
            };

            var templateModel = new QuestionnaireTemplate
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                IsLocked = false,
                Questions = new List<QuestionnaireTemplateQuestion>()

            };

            _mockUnitOfWork
                .Setup(u => u.QuestionnaireTemplate.AddAsync(request))
                .ReturnsAsync(templateModel);

            // Act
            var result = await _service.AddTemplate(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Template 1", result.Title);
            Assert.Equal("Description", result.Description);
            Assert.Empty(result.Questions);
            //_mockUnitOfWork.Verify(u => u.QuestionnaireTemplate.AddAsync(request), Times.Once);
            //_mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
        [Fact]
        public async Task GetTemplateById_ShouldReturnTemplate_WhenExists()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var template = new QuestionnaireTemplate
            {
                Id = templateId,
                Title = "Existing Template",
                Description = "Desc",
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                IsLocked = false,
                Questions = new List<QuestionnaireTemplateQuestion>()
            };

            _mockUnitOfWork
                .Setup(u => u.QuestionnaireTemplate.GetFullQuestionnaireTemplateAsync(templateId))
                .ReturnsAsync(template);

            // Act
            var result = await _service.GetTemplateById(templateId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(templateId, result.Id);
            Assert.Equal("Existing Template", result.Title);
        }

        [Fact]
        public async Task GetTemplateById_ShouldThrowItemNotFound_WhenDoesNotExist()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            _mockUnitOfWork
                .Setup(u => u.QuestionnaireTemplate.GetFullQuestionnaireTemplateAsync(templateId))
                .ReturnsAsync((QuestionnaireTemplate?)null);

            // Act & Assert
            await Assert.ThrowsAsync<SQLException.ItemNotFound>(() => _service.GetTemplateById(templateId));
        }

        [Fact]
        public async Task FinalizeTemplate_ShouldReturnFinalizedTemplate()
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
                Questions =  new List<QuestionnaireTemplateQuestion>()
            };

            var finalizedTemplate = template with { TemplateStatus = TemplateStatus.Finalized };

            _mockUnitOfWork
                .Setup(u => u.QuestionnaireTemplate.FinalizeAsync(templateId))
                .ReturnsAsync(finalizedTemplate);

            // Act
            var result = await _service.FinalizeTemplate(templateId);

            // Assert
            Assert.Equal(TemplateStatus.Finalized, result.TemplateStatus);
        }

        [Fact]
        public async Task GetTemplateBasesWithKeysetPagination_ShouldReturnPagedResult()
        {
            // Arrange
            var request = new TemplateKeysetPaginationRequest
            {
                PageSize = 10,
                Title = "Test",
                Order = TemplateOrderingOptions.TitleAsc, 
                Id = null,
                templateStatus = null

            };

            var templates = new List<QuestionnaireTemplateBase>
            {
                new() { Id = Guid.NewGuid(), Title = "Template1", CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow, IsLocked = false }
            };

            _mockUnitOfWork
                .Setup(u => u.QuestionnaireTemplate.PaginationQueryWithKeyset(
                    It.IsAny<int>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<TemplateOrderingOptions>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<TemplateStatus?>()
                ))
                .ReturnsAsync((templates, templates.Count));

            // Act
            var result = await _service.GetTemplateBasesWithKeysetPagination(request);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.TemplateBases);
            Assert.Equal(templates.Count, result.TotalCount);
            Assert.NotNull(result.QueryCursor);
        }
    }
}
