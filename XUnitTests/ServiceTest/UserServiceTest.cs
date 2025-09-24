using API.DTO.Requests.ActiveQuestionnaire;
using API.Interfaces;
using API.Services;
using Database.DTO.ActiveQuestionnaire;
using Database.DTO.User;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTests.ServiceTest
{
    public class UserServiceTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly FakeLdapService _fakeLdapService;
        private readonly UserService _service;

        public UserServiceTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var ldapSessionCache = new LdapSessionCacheService(memoryCache, loggerFactory);

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> 
                {
                    { "LDAPSettings:Host", "fakeHost" },
                    { "LDAPSettings:Port", "389" },
                    { "LDAPSettings:SA", "saUser" },
                    { "LDAPSettings:SAPassword", "saPass" },
                    { "LDAPSettings:FQDN", "fake.local" },
                    { "LDAPSettings:BaseDN", "DC=fake,DC=local" },
                    { "JWTSettings:Roles:Admin", "Admin" }
                })
                .Build();

            _fakeLdapService = new FakeLdapService(ldapSessionCache, configuration);

            _service = new UserService(_fakeLdapService, _mockUnitOfWork.Object);
        }

        [Fact]
        public void GetStudentsInGroup_ShouldReturnEmptyList_WhenNoStudents()
        {
            var result = _service.GetStudentsInGroup("NonExistingGroup");

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetAllClassesWithStudentRole_ShouldReturnClassesIncludingStudent()
        {
            var result = _service.GetClassesWithStudentRole();

            Assert.Contains("Student", result);
            Assert.Contains("TestGroup1", result);
            Assert.Contains("TestGroup2", result);
        }

        [Fact]
        public async Task GetActiveQuestionnairesForStudent_ShouldReturnPaginatedResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new ActiveQuestionnaireKeysetPaginationRequestStudent
            {
                PageSize = 10,
                QueryCursor = null
            };

            var expectedList = new List<ActiveQuestionnaireBase>
    {
        new ActiveQuestionnaireBase
        {
            Id = Guid.NewGuid(),
            Title = "Test Questionnaire",
            ActivatedAt = DateTime.UtcNow,
            Student = new UserBase { UserName = "Student1", FullName = "Student One" },
            Teacher = new UserBase { UserName = "Teacher1", FullName = "Teacher One" },
            StudentCompletedAt = null,
            TeacherCompletedAt = null
        }
    };

            _mockUnitOfWork.Setup(u => u.ActiveQuestionnaire.PaginationQueryWithKeyset(
                It.IsAny<int>(),                          // pageSize
                    It.IsAny<Database.Enums.ActiveQuestionnaireOrderingOptions>(), // order
                    It.IsAny<Guid?>(),                        
                    It.IsAny<DateTime?>(),                    
                    It.IsAny<string>(),                       // title
                    It.IsAny<string>(),                       // student
                    It.IsAny<string>(),                       // teacher
                    It.IsAny<Guid?>(),                        // idQuery
                    userId,                                   // userId
                    It.IsAny<bool>(),                          // onlyStudentCompleted
                    It.IsAny<bool>()                           // onlyTeacherCompleted (optional)
                )).ReturnsAsync((expectedList, expectedList.Count));

            // Act
            var result = await _service.GetActiveQuestionnairesForStudent(request, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.ActiveQuestionnaireBases);
            Assert.Equal("Test Questionnaire", result.ActiveQuestionnaireBases[0].Title);
        }
    }
}

