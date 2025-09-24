//using API.DTO.LDAP;
//using API.DTO.Requests.ActiveQuestionnaire;
//using API.Interfaces;
//using API.Services;
//using Database.DTO.ActiveQuestionnaire;
//using Database.DTO.QuestionnaireTemplate;
//using Database.DTO.User;
//using Database.Enums;
//using Database.Models;
//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace XUnitTests.ServiceTest
//{
//    //public class FakeLdapService : LdapService
//    //{
        
//    //    public FakeLdapService(LdapSessionCacheService ldapSessionCache, IConfiguration configuration)
//    //        : base(configuration, ldapSessionCache) 
//    //    { }

//    //    public new void Authenticate(string username, string password) { }
//    //    public new void Authenticate() { }
//    //    public new List<LdapUserDTO> GetStudentsInGroup(string groupName) => new List<LdapUserDTO>();
//    //    public new List<string> GetAllGroups() => new List<string> { "TestGroup1", "TestGroup2" };
//    //    public new string GetBaseDN() => "DC=fake,DC=local";
//    //}

//    public class ActiveQuestionnaireServiceTest
//    {
//        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
//        private readonly ActiveQuestionnaireService _service;
//        private readonly IConfiguration _configuration;


//        public ActiveQuestionnaireServiceTest()
//        {
//            _mockUnitOfWork = new Mock<IUnitOfWork>();

//            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
//            var memoryCache = new MemoryCache(new MemoryCacheOptions());
//            var ldapSessionCache = new LdapSessionCacheService(memoryCache, loggerFactory);

//            _configuration = new ConfigurationBuilder()
//                .AddInMemoryCollection(new Dictionary<string, string>
//                {
//            { "LDAPSettings:Host", "fakeHost" },
//            { "LDAPSettings:Port", "389" },
//            { "LDAPSettings:SA", "saUser" },
//            { "LDAPSettings:SAPassword", "saPass" },
//            { "LDAPSettings:FQDN", "fake.local" },
//            { "LDAPSettings:BaseDN", "DC=fake,DC=local" },
//            { "JWTSettings:Roles:Admin", "Admin" }
//                })
//                .Build();

//            var fakeLdap = new FakeLdapService(ldapSessionCache, _configuration);

//            _service = new ActiveQuestionnaireService(
//                _mockUnitOfWork.Object,
//                fakeLdap,
//                _configuration
//            );
//        }
//        #region FetchActiveQuestionnaireBases
//        [Fact]
//        public async Task FetchActiveQuestionnaireBases_ShouldReturnPaginatedResult_WhenItemsExist()
//        {
//            var request = new ActiveQuestionnaireKeysetPaginationRequestFull { PageSize = 10 };

//            var expectedList = new List<ActiveQuestionnaireBase>
//            {
//                new ActiveQuestionnaireBase
//                {
//                    Id = Guid.NewGuid(),
//                    Title = "Test1",
//                    ActivatedAt = DateTime.UtcNow,
//                    Student = new UserBase { UserName = "Student1", FullName = "Student One" },
//                    Teacher = new UserBase { UserName = "Teacher1", FullName = "Teacher One" },
//                    StudentCompletedAt = null,
//                    TeacherCompletedAt = null
//                },
//                new ActiveQuestionnaireBase
//                {
//                    Id = Guid.NewGuid(),
//                    Title = "Test2",
//                    ActivatedAt = DateTime.UtcNow,
//                    Student = new UserBase { UserName = "Student2", FullName = "Student Two" },
//                    Teacher = new UserBase { UserName = "Teacher2", FullName = "Teacher Two" },
//                    StudentCompletedAt = null,
//                    TeacherCompletedAt = null
//                }
//            };

//            _mockUnitOfWork.Setup(u => u.ActiveQuestionnaire.PaginationQueryWithKeyset(
//                It.IsAny<int>(),
//                It.IsAny<ActiveQuestionnaireOrderingOptions>(),
//                It.IsAny<Guid?>(),
//                It.IsAny<DateTime?>(),
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<Guid?>(),
//                It.IsAny<Guid?>(),
//                It.IsAny<bool>(),
//                It.IsAny<bool>()))
//                .ReturnsAsync((expectedList, expectedList.Count));

//            var result = await _service.FetchActiveQuestionnaireBases(request);

//            Assert.NotNull(result);
//            Assert.Equal(expectedList.Count, result.ActiveQuestionnaireBases.Count);
//            Assert.Equal(expectedList[0].Title, result.ActiveQuestionnaireBases[0].Title);
//        }

//        [Fact]
//        public async Task FetchActiveQuestionnaireBases_ShouldReturnEmpty_WhenNoItemsExist()
//        {
//            var request = new ActiveQuestionnaireKeysetPaginationRequestFull { PageSize = 10 };
//            _mockUnitOfWork.Setup(u => u.ActiveQuestionnaire.PaginationQueryWithKeyset(
//                It.IsAny<int>(),
//                It.IsAny<ActiveQuestionnaireOrderingOptions>(),
//                It.IsAny<Guid?>(),
//                It.IsAny<DateTime?>(),
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<Guid?>(),
//                It.IsAny<Guid?>(),
//                It.IsAny<bool>(),
//                It.IsAny<bool>()))
//                .ReturnsAsync((new List<ActiveQuestionnaireBase>(), 0));

//            var result = await _service.FetchActiveQuestionnaireBases(request);

//            Assert.NotNull(result);
//            Assert.Empty(result.ActiveQuestionnaireBases);
//        }
//        #endregion

//        #region ActivateQuestionnaireGroup
//        [Fact]
//        public async Task ActivateQuestionnaireGroup_ShouldReturnGroupResult_WhenSuccessful()
//        {
//            var request = new ActivateQuestionnaireGroup
//            {
//                TemplateId = Guid.NewGuid(),
//                Name = "Group 1",
//                StudentIds = new List<Guid> { Guid.NewGuid() },
//                TeacherIds = new List<Guid> { Guid.NewGuid() }
//            };

//            _mockUnitOfWork.Setup(u => u.QuestionnaireGroup.AddAsync(It.IsAny<QuestionnaireGroupModel>()))
//                .Returns(Task.CompletedTask);

//            _mockUnitOfWork.Setup(u => u.ActiveQuestionnaire.ActivateQuestionnaireAsync(
//                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
//                .ReturnsAsync(new ActiveQuestionnaire
//                {
//                    Id = Guid.NewGuid(),
//                    Title = "Questionnaire",
//                    Description = "Test description",
//                    ActivatedAt = DateTime.UtcNow,
//                    Student = new UserBase { UserName = "S1", FullName = "Student 1" },
//                    Teacher = new UserBase { UserName = "T1", FullName = "Teacher 1" },
//                    StudentCompletedAt = null,
//                    TeacherCompletedAt = null,
//                    Questions = new List<QuestionnaireTemplateQuestion>()
//                });

//            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

//            var result = await _service.ActivateQuestionnaireGroup(request);

//            Assert.NotNull(result);
//            Assert.Equal(request.Name, result.Name);
//            Assert.Single(result.Questionnaires);
//            Assert.Equal("Questionnaire", result.Questionnaires[0].Title);
//        }
//        #endregion

//        #region HasUserSubmittedAnswer
//        [Fact]
//        public async Task HasUserSubmittedAnswer_ShouldReturnTrue_WhenAnswerExists()
//        {
//            var userId = Guid.NewGuid();
//            var activeQId = Guid.NewGuid();
//            _mockUnitOfWork.Setup(u => u.ActiveQuestionnaire.HasUserSubmittedAnswer(userId, activeQId))
//                .ReturnsAsync(true);

//            var result = await _service.HasUserSubmittedAnswer(userId, activeQId);

//            Assert.True(result);
//        }

//        [Fact]
//        public async Task HasUserSubmittedAnswer_ShouldReturnFalse_WhenAnswerDoesNotExist()
//        {
//            var userId = Guid.NewGuid();
//            var activeQId = Guid.NewGuid();
//            _mockUnitOfWork.Setup(u => u.ActiveQuestionnaire.HasUserSubmittedAnswer(userId, activeQId))
//                .ReturnsAsync(false);

//            var result = await _service.HasUserSubmittedAnswer(userId, activeQId);

//            Assert.False(result);
//        }
//        #endregion
//    }
//}

