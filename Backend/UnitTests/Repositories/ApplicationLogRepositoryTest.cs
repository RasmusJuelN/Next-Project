using Database;
using Database.DTO.ApplicationLog;
using Database.Models;
using Database.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace UnitTests.Repositories
{
    public class ApplicationLogRepositoryTest
    {
        private readonly DbContextOptions<Context> _options;
        private readonly Context _context;
        private readonly ApplicationLogRepository _repo;
        public ApplicationLogRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<Context>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            _context = new Context(_options);
            _repo = new ApplicationLogRepository(_context, NullLoggerFactory.Instance);
        }
        [Fact]
        public async Task AddAsync_ShouldAddSingleLog()
        {
            // Arrange
            var log = new ApplicationLog
            {
                Message = "Test log",
                Category = "Test",
                LogLevel = LogLevel.Information,   
                EventId = 1,                      
                Timestamp = DateTime.UtcNow         
            };

            // Act
            await _repo.AddAsync(log);
            await _context.SaveChangesAsync();

            // Assert
            var savedLog = await _context.ApplicationLogs.FirstOrDefaultAsync();
            Assert.NotNull(savedLog);
            Assert.Equal(log.Message, savedLog.Message);
            Assert.Equal(log.Category, savedLog.Category);
            Assert.Equal(log.LogLevel, savedLog.LogLevel);
            Assert.Equal(log.EventId, savedLog.EventId);
        }
        [Fact]
        public async Task AddRangeAsync_ShouldAddMultipleLogs()
        {
            // Arrange
            var logs = new List<ApplicationLog>
    {
        new ApplicationLog
        {
            Message = "Log1",
            Category = "Cat1",
            LogLevel = LogLevel.Information,  // required
            EventId = 1,                      // required
            Timestamp = DateTime.UtcNow       // required, not CreatedAt
        },
        new ApplicationLog
        {
            Message = "Log2",
            Category = "Cat2",
            LogLevel = LogLevel.Warning,
            EventId = 2,
            Timestamp = DateTime.UtcNow
        }
    };

            // Act
            await _repo.AddRangeAsync(logs);
            await _context.SaveChangesAsync();

            // Assert
            var savedLogs = await _context.ApplicationLogs.ToListAsync();
            Assert.Equal(2, savedLogs.Count);
            Assert.Contains(savedLogs, l => l.Message == "Log1" && l.Category == "Cat1");
            Assert.Contains(savedLogs, l => l.Message == "Log2" && l.Category == "Cat2");
        }

        [Fact]
        public async Task GetLogCategoriesAsync_ShouldReturnDistinctCategories()
        {
            // Arrange
            _context.ApplicationLogs.AddRange(
                new ApplicationLogsModel
                {
                    Message = "Log1",
                    Category = "Cat1",
                    LogLevel = LogLevel.Information,
                    EventId = 1,
                    Timestamp = DateTime.UtcNow
                },
                new ApplicationLogsModel
                {
                    Message = "Log2",
                    Category = "Cat2",
                    LogLevel = LogLevel.Warning,
                    EventId = 2,
                    Timestamp = DateTime.UtcNow
                },
                new ApplicationLogsModel
                {
                    Message = "Log3",
                    Category = "Cat1", // duplicate category
                    LogLevel = LogLevel.Error,
                    EventId = 3,
                    Timestamp = DateTime.UtcNow
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var categories = await _repo.GetLogCategoriesAsync();

            // Assert
            Assert.Equal(2, categories.Count);
            Assert.Contains("Cat1", categories);
            Assert.Contains("Cat2", categories);
        }

    }
}
