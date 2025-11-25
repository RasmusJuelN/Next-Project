using API.DTO.LDAP;
using API.DTO.Requests.ActiveQuestionnaire;
using API.Exceptions;
using API.Interfaces;
using API.Services;
using Database.DTO.ActiveQuestionnaire;
using Database.DTO.QuestionnaireTemplate;
using Database.DTO.User;
using Database.Enums;
using Microsoft.Extensions.Configuration;
using Moq;
using Novell.Directory.Ldap;

namespace UnitTests.Services
{
    public class ActiveQuestionnaireServiceTests
    {
    
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IAuthenticationBridge> _authMock;
        //private readonly Mock<IConfiguration> _configMock;
        private readonly IConfiguration _config;
        private readonly ActiveQuestionnaireService _service;

        public ActiveQuestionnaireServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _authMock = new Mock<IAuthenticationBridge>();
            // Use real configuration so ConfigurationBinder won't throw
            var inMemorySettings = new Dictionary<string, string?>
            {
                ["LDAP:SA"] = "sa",
                ["LDAP:SAPassword"] = "password",
                ["JWT:Roles:Student"] = "Student",
                ["JWT:Roles:Teacher"] = "Teacher"
                //["JWT:Roles"] = "Student,Teacher"
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            //Mock authentication bridge
            _authMock.Setup(a => a.Authenticate(It.IsAny<string>(), It.IsAny<string>()));
            _authMock.Setup(a => a.IsConnected()).Returns(true);
            _authMock.Setup(a => a.SearchId<BasicUserInfo>(It.IsAny<string>()))
                .Returns((string id) => new BasicUserInfo
                {
                    Username = new LdapAttribute("username", $"user_{id}"),
                    Name = new LdapAttribute("name", $"User {id}"),
                    MemberOf = new LdapAttribute("memberof", "Student")
                });

            _service = new ActiveQuestionnaireService(_unitOfWorkMock.Object, _authMock.Object, _config);
        }

        [Fact]
        public async Task FetchActiveQuestionnaireBases_ShouldReturnResults()
        {
            var request = new ActiveQuestionnaireKeysetPaginationRequestFull
            {
                PageSize = 5,
                Order = ActiveQuestionnaireOrderingOptions.TitleAsc
            };

            var fakeQ = new List<ActiveQuestionnaireBase>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Q1",
                    ActivatedAt = DateTime.UtcNow,
                    Student = new UserBase
                    {
                        UserName = "student1",
                        FullName = "Student One"
                    },
                    Teacher = new UserBase
                    {
                        UserName = "teacher1",
                        FullName = "Teacher One"
                    },
                    StudentCompletedAt = null,
                    TeacherCompletedAt = null
                }
            };
            _unitOfWorkMock.Setup(u => u.ActiveQuestionnaire.PaginationQueryWithKeyset(
                It.IsAny<int>(),                                  // amount
                It.IsAny<ActiveQuestionnaireOrderingOptions>(),   // sortOrder
                It.IsAny<Guid?>(),                                // cursorIdPosition
                It.IsAny<DateTime?>(),                            // cursorActivatedAtPosition
                It.IsAny<string>(),                               // titleQuery
                It.IsAny<string>(),                               // student
                It.IsAny<string>(),                               // teacher
                It.IsAny<Guid?>(),                                // idQuery
                It.IsAny<Guid?>(),                                // userId
                It.IsAny<bool>(),                                 // onlyStudentCompleted
                It.IsAny<bool>(),                                 // onlyTeacherCompleted
                It.IsAny<bool>(),                                 // pendingStudent
                It.IsAny<bool>()                                  // pendingTeacher
            )).ReturnsAsync((fakeQ, fakeQ.Count));

            var result = await _service.FetchActiveQuestionnaireBases(request);

            Assert.NotNull(result);
            Assert.Single(result.ActiveQuestionnaireBases);
            Assert.Equal(fakeQ.Count, result.TotalCount);
        }

        [Fact]
        public async Task ActivateTemplate_ShouldCreateUsersAndQuestionnaire()
        {
            var request = new ActivateQuestionnaire
            {
                TemplateId = Guid.NewGuid(),
                StudentIds = new List<Guid> { Guid.NewGuid() },
                TeacherIds = new List<Guid> { Guid.NewGuid() }
            };

            _unitOfWorkMock.Setup(u => u.User.UserExists(It.IsAny<Guid>())).Returns(false);
            _unitOfWorkMock.Setup(u => u.User.AddStudentAsync(It.IsAny<UserAdd>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.User.AddTeacherAsync(It.IsAny<UserAdd>())).Returns(Task.CompletedTask);

            // Mock student
            _authMock.Setup(a => a.SearchId<BasicUserInfo>(It.Is<string>(id => request.StudentIds.Contains(Guid.Parse(id)))))
                .Returns((string id) => new BasicUserInfo
                {
                    Username = new LdapAttribute("username", $"user_{id}"),
                    Name = new LdapAttribute("name", $"Student {id}"),
                    MemberOf = new LdapAttribute("memberof", "Student")
                });

            // Mock teacher
            _authMock.Setup(a => a.SearchId<BasicUserInfo>(It.Is<string>(id => request.TeacherIds.Contains(Guid.Parse(id)))))
                .Returns((string id) => new BasicUserInfo
                {
                    Username = new LdapAttribute("username", $"user_{id}"),
                    Name = new LdapAttribute("name", $"Teacher {id}"),
                    MemberOf = new LdapAttribute("memberof", "Teacher")
                });

            // Mock questionnaire activation
            _unitOfWorkMock.Setup(u => u.ActiveQuestionnaire.ActivateQuestionnaireAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns((Guid templateId, Guid studentId, Guid teacherId, Guid groupId) =>
                    Task.FromResult(new ActiveQuestionnaire
                    {
                        Id = Guid.NewGuid(),
                        Title = "Activated Q",
                        ActivatedAt = DateTime.UtcNow,
                        Student = new UserBase { UserName = "student1", FullName = "Student One" },
                        Teacher = new UserBase { UserName = "teacher1", FullName = "Teacher One" },
                        StudentCompletedAt = null,
                        TeacherCompletedAt = null,
                        Questions = new List<QuestionnaireTemplateQuestion>
                        {
                    new QuestionnaireTemplateQuestion
                    {
                        Id = 1,
                        Prompt = "Sample question",
                        AllowCustom = false,
                        Options = new List<QuestionnaireTemplateOption>
                        {
                            new QuestionnaireTemplateOption { Id = 1, OptionValue = 1, DisplayText = "" },
                            new QuestionnaireTemplateOption { Id = 2, OptionValue = 2, DisplayText = "Option 2" }
                        }
                    }
                        }
                    }));

            var result = await _service.ActivateTemplate(request);

            _authMock.Verify(a => a.Authenticate(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.User.AddStudentAsync(It.IsAny<UserAdd>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.User.AddTeacherAsync(It.IsAny<UserAdd>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.AtLeastOnce);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Activated Q", result[0].Title);
        }


        [Fact]
        public async Task SubmitAnswers_ShouldSave_WhenNotSubmitted()
        {
            var userId = Guid.NewGuid();
            var qId = Guid.NewGuid();
            var submission = new AnswerSubmission { Answers = new List<Answer>() };

            _unitOfWorkMock.Setup(u => u.ActiveQuestionnaire.HasUserSubmittedAnswer(userId, qId)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.ActiveQuestionnaire.AddAnswers(qId, userId, submission)).Returns(Task.CompletedTask);

            await _service.SubmitAnswers(qId, userId, submission);

            _unitOfWorkMock.Verify(u => u.ActiveQuestionnaire.AddAnswers(qId, userId, submission), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task SubmitAnswers_ShouldThrow_WhenAlreadySubmitted()
        {
            var userId = Guid.NewGuid();
            var qId = Guid.NewGuid();
            var submission = new AnswerSubmission { Answers = new List<Answer>() };

            _unitOfWorkMock.Setup(u => u.ActiveQuestionnaire.HasUserSubmittedAnswer(userId, qId)).ReturnsAsync(true);

            await Assert.ThrowsAsync<HttpResponseException>(() => _service.SubmitAnswers(qId, userId, submission));

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task IsActiveQuestionnaireComplete_ShouldCallRepo()
        {
            var qId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.ActiveQuestionnaire.IsActiveQuestionnaireComplete(qId, userId)).ReturnsAsync(true);

            var result = await _service.IsActiveQuestionnaireComplete(qId, userId);

            Assert.True(result);
        }

        [Fact]
        public async Task GetOldestActiveQuestionnaireForUser_ShouldCallRepo()
        {
            var userId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.User.GetIdOfOldestActiveQuestionnaire(userId)).ReturnsAsync(Guid.NewGuid());

            var result = await _service.GetOldestActiveQuestionnaireForUser(userId);

            Assert.NotNull(result);
        }
    }

}


