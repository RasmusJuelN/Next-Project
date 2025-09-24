using Database;
using Database.DTO.QuestionnaireTemplate;
using Database.Models;
using Database.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTests.ReposTest
{
    public class QuestionnaireTemplateRepositoryTest
    {
        private readonly DbContextOptions<Context> _options;
        private readonly Context _context;
        private readonly QuestionnaireTemplateRepository _repo;


        public QuestionnaireTemplateRepositoryTest() 
        {
            _options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new Context(_options);
            _repo = new QuestionnaireTemplateRepository(_context, NullLoggerFactory.Instance);
        }


        private QuestionnaireTemplateRepository CreateRepo()
        {
            var context = new Context(_options);
            return new QuestionnaireTemplateRepository(context, NullLoggerFactory.Instance);
        }

        [Fact]
        public async Task AddAsync_ShouldAddTemplate()
        {
            // Arrange
            var repo = CreateRepo();
            var templateAdd = new QuestionnaireTemplateAdd
            {
                Title = "Template 1",
                Description = "Description 1",
                Questions = new List<QuestionnaireQuestionAdd>() // DTO type
            };

            // Act
            var dto = await repo.AddAsync(templateAdd);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal("Template 1", dto.Title);
            Assert.Equal("Description 1", dto.Description);
            Assert.Empty(dto.Questions); // should be empty
            Assert.Equal(TemplateStatus.Draft, dto.TemplateStatus); // default status set by repo
            Assert.True(dto.CreatedAt <= DateTime.UtcNow); // ensure timestamp is set
            Assert.True(dto.LastUpdated <= DateTime.UtcNow); // ensure last updated is set
        }


        [Fact]
        public async Task GetFullQuestionnaireTemplateAsync_ShouldReturnTemplateWithQuestions()
        {
            // Arrange
            var context = new Context(_options);
            var template = new QuestionnaireTemplateModel
            {
                Id = Guid.NewGuid(),
                Title = "Template 2",
                Description = "Test",
                CreatedAt = DateTime.UtcNow,
                LastUpated = DateTime.UtcNow,
                Questions = new List<QuestionnaireQuestionModel>
                {
                    new() { Id = 1, Prompt = "Q1", AllowCustom = true, Options = new List<QuestionnaireOptionModel>
                        {
                            new() { Id = 1, OptionValue = 1, DisplayText = "Option 1" }
                        }
                    }
                }
            };
            context.QuestionnaireTemplates.Add(template);
            await context.SaveChangesAsync();

            var repo = CreateRepo();

            // Act
            var result = await repo.GetFullQuestionnaireTemplateAsync(template.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result!.Questions);
            Assert.Equal("Q1", result.Questions[0].Prompt);
            Assert.Single(result.Questions[0].Options);
            Assert.Equal("Option 1", result.Questions[0].Options[0].DisplayText);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveTemplate()
        {
            // Arrange
            var context = new Context(_options);
            var template = new QuestionnaireTemplateModel
            {
                Id = Guid.NewGuid(),
                Title = "Template 3",
                Description = "Test",
                CreatedAt = DateTime.UtcNow,
                LastUpated = DateTime.UtcNow
            };
            context.QuestionnaireTemplates.Add(template);
            await context.SaveChangesAsync();

            // var repo = CreateRepo();
            var repo = new QuestionnaireTemplateRepository(context, NullLoggerFactory.Instance);


            // Act
            await repo.DeleteAsync(template.Id);

            // Assert
            var exists = await context.QuestionnaireTemplates.FindAsync(template.Id);
            Assert.Null(exists); 
        }

        [Fact]
        public async Task FinalizeAsync_ShouldSetTemplateStatusFinalized()
        {
            // Arrange
            var context = new Context(_options);
            var template = new QuestionnaireTemplateModel
            {
                Id = Guid.NewGuid(),
                Title = "Template 4",
                Description = "Test",
                CreatedAt = DateTime.UtcNow,
                LastUpated = DateTime.UtcNow,
                TemplateStatus = TemplateStatus.Draft
            };
            context.QuestionnaireTemplates.Add(template);
            await context.SaveChangesAsync();

            var repo = CreateRepo();

            // Act
            var dto = await repo.FinalizeAsync(template.Id);

            // Assert
            Assert.Equal(TemplateStatus.Finalized, dto.TemplateStatus);
        }
    }
}
