//using API.Controllers;
//using API.DTO.Requests.ActiveQuestionnaire;
//using API.DTO.Responses.ActiveQuestionnaire;
//using API.Exceptions;
//using API.Services;
//using Database.DTO.ActiveQuestionnaire;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace UnitTests.Controllers
//{
//    public class ActiveQuestionnaireControllerTest
//    {
//        private readonly Mock<ActiveQuestionnaireService> _mockService;
//        private readonly Mock<ILoggerFactory> _mockLoggerFactory;
//        private readonly ActiveQuestionnaireController _controller;

//        public ActiveQuestionnaireControllerTest() 
//        {
//            // Mock dependencies
//            _mockService = new Mock<ActiveQuestionnaireService>(null, null, null, null, null, null);
//            _mockLoggerFactory = new Mock<ILoggerFactory>();
//            _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<Type>())).Returns(Mock.Of<ILogger>());

//            // Create controller with mocked service and logger
//            _controller = new ActiveQuestionnaireController(_mockService.Object, _mockLoggerFactory.Object);
//        }

//        // ✅ Utility: fake authenticated user
//        private void SetFakeUser(Guid userId, string role = "Student")
//        {
//            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
//            {
//                new Claim("sub", userId.ToString()),
//                new Claim(ClaimTypes.Role, role)
//            }, "mock"));

//            _controller.ControllerContext = new ControllerContext
//            {
//                HttpContext = new DefaultHttpContext { User = user }
//            };
//        }

//        // Utility: clear user (simulate unauthenticated)
//        private void ClearUser()
//        {
//            _controller.ControllerContext = new ControllerContext
//            {
//                HttpContext = new DefaultHttpContext()
//            };
//        }

//        // === TESTS ===

//        [Fact]
//        public async Task GetActiveQuestionnaires_ReturnsOk()
//        {
//            var req = new ActiveQuestionnaireKeysetPaginationRequestFull(); 
//            var expected = new ActiveQuestionnaireKeysetPaginationResultAdmin();

//            _mockService.Setup(s => s.FetchActiveQuestionnaireBases(req))
//                        .ReturnsAsync(expected);

//            var result = await _controller.GetActiveQuestionnaires(req);

//            var ok = Assert.IsType<OkObjectResult>(result.Result);
//            Assert.Equal(expected, ok.Value);
//        }

//        [Fact]
//        public async Task ActivateQuestionnaire_ReturnsOk()
//        {
//            var req = new ActivateQuestionnaire();
//            var fake = new List<ActiveQuestionnaire> { new() { Id = Guid.NewGuid() } };

//            _mockService.Setup(s => s.ActivateTemplate(req)).ReturnsAsync(fake);

//            var result = await _controller.ActivateQuestionnaire(req);

//            var ok = Assert.IsType<OkObjectResult>(result.Result);
//            Assert.Equal(fake, ok.Value);
//        }

//        [Fact]
//        public async Task GetGroup_ReturnsNotFound_WhenGroupIsNull()
//        {
//            var gid = Guid.NewGuid();
//            _mockService.Setup(s => s.GetQuestionnaireGroup(gid))
//                        .ReturnsAsync((QuestionnaireGroupResult)null);

//            var result = await _controller.GetGroup(gid);

//            Assert.IsType<NotFoundResult>(result.Result);
//        }

//        [Fact]
//        public async Task GetGroup_ReturnsOk_WhenFound()
//        {
//            var gid = Guid.NewGuid();
//            var fake = new QuestionnaireGroupResult();
//            _mockService.Setup(s => s.GetQuestionnaireGroup(gid)).ReturnsAsync(fake);

//            var result = await _controller.GetGroup(gid);

//            var ok = Assert.IsType<OkObjectResult>(result.Result);
//            Assert.Equal(fake, ok.Value);
//        }

//        [Fact]
//        public async Task CheckIfUserHasActiveQuestionnaire_ReturnsOk()
//        {
//            var uid = Guid.NewGuid();
//            SetFakeUser(uid);
//            var expected = Guid.NewGuid();

//            _mockService.Setup(s => s.GetOldestActiveQuestionnaireForUser(uid))
//                        .ReturnsAsync(expected);

//            var result = await _controller.CheckIfUserHasActiveQuestionnaire();

//            Assert.Equal(expected, result.Value);
//        }

//        [Fact]
//        public async Task CheckIfUserHasActiveQuestionnaire_ReturnsUnauthorized_WhenNoUser()
//        {
//            ClearUser();

//            var result = await _controller.CheckIfUserHasActiveQuestionnaire();

//            Assert.IsType<UnauthorizedResult>(result.Result);
//        }

//        [Fact]
//        public async Task SubmitQuestionnaireAnswer_ReturnsOk()
//        {
//            var uid = Guid.NewGuid();
//            SetFakeUser(uid);
//            var qid = Guid.NewGuid();
//            var submission = new AnswerSubmission();

//            _mockService.Setup(s => s.SubmitAnswers(qid, uid, submission))
//                        .Returns(Task.CompletedTask);

//            var result = await _controller.SubmitQuestionnaireAnswer(qid, submission);

//            Assert.IsType<OkResult>(result);
//        }

//        [Fact]
//        public async Task SubmitQuestionnaireAnswer_ReturnsUnauthorized_WhenNoUser()
//        {
//            ClearUser();
//            var qid = Guid.NewGuid();
//            var submission = new AnswerSubmission();

//            var result = await _controller.SubmitQuestionnaireAnswer(qid, submission);

//            Assert.IsType<UnauthorizedResult>(result);
//        }

//        [Fact]
//        public async Task SubmitQuestionnaireAnswer_ReturnsHttpResponseError()
//        {
//            var uid = Guid.NewGuid();
//            SetFakeUser(uid);
//            var qid = Guid.NewGuid();
//            var submission = new AnswerSubmission();

//            _mockService.Setup(s => s.SubmitAnswers(qid, uid, submission))
//                        .ThrowsAsync(new HttpResponseException(System.Net.HttpStatusCode.BadRequest, "Invalid"));

//            var result = await _controller.SubmitQuestionnaireAnswer(qid, submission);

//            var status = Assert.IsType<ObjectResult>(result);
//            Assert.Equal(400, status.StatusCode);
//        }

//        [Fact]
//        public async Task CheckIfQuestionnaireAnswered_ReturnsOk()
//        {
//            var uid = Guid.NewGuid();
//            SetFakeUser(uid);
//            var qid = Guid.NewGuid();

//            _mockService.Setup(s => s.HasUserSubmittedAnswer(uid, qid))
//                        .ReturnsAsync(true);

//            var result = await _controller.CheckIfQuestionnaireAnswered(qid);

//            Assert.True(result.Value);
//        }

//        [Fact]
//        public async Task CheckIfQuestionnaireAnswered_ReturnsUnauthorized_WhenNoUser()
//        {
//            ClearUser();
//            var result = await _controller.CheckIfQuestionnaireAnswered(Guid.NewGuid());
//            Assert.IsType<UnauthorizedResult>(result.Result);
//        }

//        [Fact]
//        public async Task CheckIfQuestionnaireCompleted_ReturnsOk()
//        {
//            var uid = Guid.NewGuid();
//            SetFakeUser(uid, "Teacher");
//            var qid = Guid.NewGuid();

//            _mockService.Setup(s => s.IsActiveQuestionnaireComplete(qid, uid))
//                        .ReturnsAsync(true);

//            var result = await _controller.CheckifQuestionnaireCompleted(qid);

//            Assert.True(result.Value);
//        }

//        [Fact]
//        public async Task GetAnonymisedResponses_ReturnsOk()
//        {
//            var req = new AnonymisedResponsesRequest
//            {
//                QuestionnaireId = Guid.NewGuid(),
//                Users = new List<Guid>(),
//                Groups = new List<Guid>()
//            };

//            var expected = new SurveyResponseSummary();
//            _mockService.Setup(s => s.GetAnonymisedResponses(req.QuestionnaireId, req.Users, req.Groups))
//                        .ReturnsAsync(expected);

//            var result = await _controller.GetAnonymisedResponses(req);

//            var ok = Assert.IsType<OkObjectResult>(result.Result);
//            Assert.Equal(expected, ok.Value);
//        }

//        [Fact]
//        public async Task GetAnonymisedResponses_Returns500_OnException()
//        {
//            var req = new AnonymisedResponsesRequest
//            {
//                QuestionnaireId = Guid.NewGuid()
//            };

//            _mockService.Setup(s => s.GetAnonymisedResponses(It.IsAny<Guid>(), It.IsAny<List<Guid>>(), It.IsAny<List<Guid>>()))
//                        .ThrowsAsync(new Exception("boom"));

//            var result = await _controller.GetAnonymisedResponses(req);

//            var obj = Assert.IsType<ObjectResult>(result.Result);
//            Assert.Equal(500, obj.StatusCode);
//        }


//    }
//}

