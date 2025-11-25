using API.Controllers;
using API.DTO.Requests.ActiveQuestionnaire;
using API.DTO.Responses.ActiveQuestionnaire;
using API.Exceptions;
using API.Interfaces;
using Database.DTO.ActiveQuestionnaire;
using Database.DTO.QuestionnaireTemplate;
using Database.DTO.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace UnitTests.Controllers
{
    public class ActiveQuestionnaireControllerTest
    {
        private readonly Mock<IActiveQuestionnaireService> _mockService;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ActiveQuestionnaireController _controller;

        public ActiveQuestionnaireControllerTest()
        {
            _mockService = new Mock<IActiveQuestionnaireService>();
            _loggerFactory = LoggerFactory.Create(builder => builder.AddFilter(_ => false));
            _controller = new ActiveQuestionnaireController(_mockService.Object, _loggerFactory);
        }

        // Utility methods
        private void SetFakeUser(Guid userId, string role = "Student")
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim("sub", userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        private void ClearUser()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task GetActiveQuestionnaires_ReturnsOk()
        {
            var req = new ActiveQuestionnaireKeysetPaginationRequestFull
            {
                PageSize = 5,
                Order = Database.Enums.ActiveQuestionnaireOrderingOptions.ActivatedAtDesc,
                Teacher = null,
                Student = null,
                FilterStudentCompleted = false,
                FilterTeacherCompleted = false
            };

            var expected = new ActiveQuestionnaireKeysetPaginationResultAdmin
            {
                ActiveQuestionnaireBases = new List<ActiveQuestionnaireBase>()
            };

            _mockService.Setup(s => s.FetchActiveQuestionnaireBases(req))
                        .ReturnsAsync(expected);

            var result = await _controller.GetActiveQuestionnaires(req);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, ok.Value);
        }
        [Fact]
        public async Task ActivateQuestionnaire_ReturnsOk()
        {
            // Arrange
            var req = new ActivateQuestionnaire
            {
                TemplateId = Guid.NewGuid(),
                StudentIds = new List<Guid> { Guid.NewGuid() },
                TeacherIds = new List<Guid> { Guid.NewGuid() }
            };

            var fake = new List<ActiveQuestionnaire>
            {
                new ActiveQuestionnaire
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Questionnaire",
                    Description = "Demo questionnaire for testing",
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
                    ActivatedAt = DateTime.UtcNow,
                    StudentCompletedAt = null,
                    TeacherCompletedAt = null,
                    Questions = new List<QuestionnaireTemplateQuestion>
                    {
                        new QuestionnaireTemplateQuestion
                        {
                            Id = 1,
                            Prompt = "Sample question?",
                            AllowCustom = true,
                            Options = new List<QuestionnaireTemplateOption>
                            {
                                new QuestionnaireTemplateOption
                                {
                                    Id = 1,
                                    OptionValue = 10,
                                    DisplayText = "Option 1"
                                },
                                new QuestionnaireTemplateOption
                                {
                                    Id = 2,
                                    OptionValue = 20,
                                    DisplayText = "Option 2"
                                }
                            }
                        }
                    }
                }
            };

            _mockService
                .Setup(s => s.ActivateTemplate(req))
                .ReturnsAsync(fake);

            // Act
            var result = await _controller.ActivateQuestionnaire(req);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(fake, ok.Value);
        }


        [Fact]
        public async Task GetGroup_ReturnsNotFound_WhenGroupIsNull()
        {
            var gid = Guid.NewGuid();
            _mockService.Setup(s => s.GetQuestionnaireGroup(gid))
                        .ReturnsAsync((QuestionnaireGroupResult?)null);

            var result = await _controller.GetGroup(gid);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetGroup_ReturnsOk_WhenFound()
        {
            var gid = Guid.NewGuid();
            var fake = new QuestionnaireGroupResult
            {
                Name = "Group 1"
            };

            _mockService.Setup(s => s.GetQuestionnaireGroup(gid)).ReturnsAsync(fake);

            var result = await _controller.GetGroup(gid);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(fake, ok.Value);
        }

        [Fact]
        public async Task CheckIfUserHasActiveQuestionnaire_ReturnsOk()
        {
            var uid = Guid.NewGuid();
            SetFakeUser(uid);
            var expected = Guid.NewGuid();

            _mockService.Setup(s => s.GetOldestActiveQuestionnaireForUser(uid))
                        .ReturnsAsync(expected);

            var result = await _controller.CheckIfUserHasActiveQuestionnaire();

            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public async Task CheckIfUserHasActiveQuestionnaire_ReturnsUnauthorized_WhenNoUser()
        {
            ClearUser();

            var result = await _controller.CheckIfUserHasActiveQuestionnaire();

            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task SubmitQuestionnaireAnswer_ReturnsOk()
        {
            // Arrange
            var uid = Guid.NewGuid();
            SetFakeUser(uid);
            var qid = Guid.NewGuid();

            var submission = new AnswerSubmission
            {
                Answers = new List<Answer>
                {
                    new Answer
                    {
                        QuestionId = 1,
                        OptionId = null,
                        CustomAnswer = "This is a test answer"
                    }
                }
            };

            _mockService
                .Setup(s => s.SubmitAnswers(qid, uid, submission))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SubmitQuestionnaireAnswer(qid, submission);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task SubmitQuestionnaireAnswer_ReturnsUnauthorized_WhenNoUser()
        {
            ClearUser();
            var qid = Guid.NewGuid();
            var submission = new AnswerSubmission
            {
                Answers = new List<Answer>
                {
                    new Answer
                    {
                        QuestionId = 1,
                        OptionId = null,
                        CustomAnswer = "This is a test answer"
                    }
                }
            };


            var result = await _controller.SubmitQuestionnaireAnswer(qid, submission);

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SubmitQuestionnaireAnswer_ReturnsHttpResponseError()
        {
            var uid = Guid.NewGuid();
            SetFakeUser(uid);
            var qid = Guid.NewGuid();
            var submission = new AnswerSubmission
            {
                Answers = new List<Answer>
                {
                    new Answer
                    {
                        QuestionId = 1,
                        OptionId = null,
                        CustomAnswer = "This is a test answer"
                    }
                }
            };

            _mockService.Setup(s => s.SubmitAnswers(qid, uid, submission))
                        .ThrowsAsync(new HttpResponseException(System.Net.HttpStatusCode.BadRequest, "Invalid"));

            var result = await _controller.SubmitQuestionnaireAnswer(qid, submission);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, status.StatusCode);
        }

        [Fact]
        public async Task CheckIfQuestionnaireAnswered_ReturnsOk()
        {
            var uid = Guid.NewGuid();
            SetFakeUser(uid);
            var qid = Guid.NewGuid();

            _mockService.Setup(s => s.HasUserSubmittedAnswer(uid, qid))
                        .ReturnsAsync(true);

            var result = await _controller.CheckIfQuestionnaireAnswered(qid);

            Assert.True(result.Value);
        }

        [Fact]
        public async Task CheckIfQuestionnaireAnswered_ReturnsUnauthorized_WhenNoUser()
        {
            ClearUser();
            var result = await _controller.CheckIfQuestionnaireAnswered(Guid.NewGuid());
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task CheckIfQuestionnaireCompleted_ReturnsOk()
        {
            var uid = Guid.NewGuid();
            SetFakeUser(uid, "Teacher");
            var qid = Guid.NewGuid();

            _mockService.Setup(s => s.IsActiveQuestionnaireComplete(qid, uid))
                        .ReturnsAsync(true);

            var result = await _controller.CheckifQuestionnaireCompleted(qid);

            Assert.True(result.Value);
        }
        [Fact]
        public async Task GetAnonymisedResponses_ReturnsOk()
        {
            // Arrange
            var req = new AnonymisedResponsesRequest
            {
                QuestionnaireId = Guid.NewGuid(),
                Users = new List<Guid>(),
                Groups = new List<Guid>()
            };

            var expected = new SurveyResponseSummary
            {
                Title = "Summary",
                AnonymisedResponseDataSet = new List<AnonymisedSurveyResults>
                {
                    new AnonymisedSurveyResults
                    {
                        DatasetTitle = "Dataset 1",
                        ParticipantCount = 5,
                        AnonymisedResponses = new List<AnonymisedResponsesQuestion>
                        {
                            new AnonymisedResponsesQuestion
                            {
                                Question = "Sample question?",
                                Answers = new List<AnonymisedResponsesAnswer>
                                {
                                    new AnonymisedResponsesAnswer
                                    {
                                        Answer = "Yes",
                                        Count = 3
                                    },
                                    new AnonymisedResponsesAnswer
                                    {
                                        Answer = "No",
                                        Count = 2
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _mockService.Setup(s => s.GetAnonymisedResponses(req.QuestionnaireId, req.Users, req.Groups))
                        .ReturnsAsync(expected);

            // Act
            var result = await _controller.GetAnonymisedResponses(req);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, ok.Value);
        }

        [Fact]
        public async Task GetAnonymisedResponses_Returns500_OnException()
        {
            var req = new AnonymisedResponsesRequest
            {
                QuestionnaireId = Guid.NewGuid(),
                Users = new List<Guid>(),
                Groups = new List<Guid>()
            };

            _mockService.Setup(s => s.GetAnonymisedResponses(It.IsAny<Guid>(), It.IsAny<List<Guid>>(), It.IsAny<List<Guid>>()))
                        .ThrowsAsync(new Exception("boom"));

            var result = await _controller.GetAnonymisedResponses(req);

            var obj = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, obj.StatusCode);
        }

        


    }
}

