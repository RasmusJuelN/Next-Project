﻿using Database;
using Database.Models;
using Database.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTests.ReposTest
{
    public class QuestionnaireGroupRepositoryTest
    {
        private readonly DbContextOptions<Context> _options;

        public QuestionnaireGroupRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        }
        [Fact]
        public async Task AddAsync_ShouldAddGroup()
        {
            // Arrange
            var group = new QuestionnaireGroupModel
            {
                GroupId = Guid.NewGuid(),
                Name = "Group 1",
                TemplateId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            await using var context = new Context(_options);
            var repo = new QuestionnaireGroupRepository(context);

            // Act
            await repo.AddAsync(group);

            // Assert
            var savedGroup = await context.QuestionnaireGroups.FindAsync(group.GroupId);
            Assert.NotNull(savedGroup);
            Assert.Equal("Group 1", savedGroup.Name);
            Assert.Equal(group.TemplateId, savedGroup.TemplateId);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnGroup()
        {
            // Arrange
            var group = new QuestionnaireGroupModel
            {
                GroupId = Guid.NewGuid(),
                Name = "Group 2",
                TemplateId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            await using var context = new Context(_options);
            context.QuestionnaireGroups.Add(group);
            await context.SaveChangesAsync();

            var repo = new QuestionnaireGroupRepository(context);

            // Act
            var result = await repo.GetByIdAsync(group.GroupId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(group.GroupId, result.GroupId);
            Assert.Equal(group.Name, result.Name);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllGroups()
        {
            // Arrange
            var group1 = new QuestionnaireGroupModel
            {
                GroupId = Guid.NewGuid(),
                Name = "Group 1",
                TemplateId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };
            var group2 = new QuestionnaireGroupModel
            {
                GroupId = Guid.NewGuid(),
                Name = "Group 2",
                TemplateId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            await using var context = new Context(_options);
            context.QuestionnaireGroups.AddRange(group1, group2);
            await context.SaveChangesAsync();

            var repo = new QuestionnaireGroupRepository(context);

            // Act
            var groups = (await repo.GetAllAsync()).ToList();

            // Assert
            Assert.Equal(2, groups.Count);
            Assert.Contains(groups, g => g.GroupId == group1.GroupId);
            Assert.Contains(groups, g => g.GroupId == group2.GroupId);
        }
    }
}
