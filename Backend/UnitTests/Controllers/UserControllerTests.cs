namespace UnitTests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;
        private readonly Guid _userId;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UserController(_mockUserService.Object);

            _userId = Guid.NewGuid();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim("sub", _userId.ToString())
                    }))
                }
            };
        }

        [Fact]
        public void UserPaginationQuery_ReturnsOk()
        {
            var request = new UserQueryPagination
            {
                PageSize = 10,
                User = "test",
                Role = API.DTO.Requests.User.Roles.Student
            };

            // Directly use LdapUserBase instead of UserBase
            var expectedResult = new UserQueryPaginationResult
            {
                UserBases = new List<LdapUserBase>
        {
            new LdapUserBase
            {
                Id = Guid.NewGuid(),
                UserName = "test",
                FullName = "Test User"
            }
        },
                SessionId = "session123",
                HasMore = false
            };

            _mockUserService.Setup(s => s.QueryLDAPUsersWithPagination(request))
                            .Returns(expectedResult);

            var result = _controller.UserPaginationQuery(request);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var users = Assert.IsType<UserQueryPaginationResult>(okResult.Value);

            Assert.Single(users.UserBases);
            Assert.Equal("session123", users.SessionId);
        }


        [Fact]
        public async Task GetPendingActiveQuestionnairesForStudent_ReturnsOk()
        {
            var activeBases = new List<ActiveQuestionnaireBase>
            {
                new ActiveQuestionnaireBase
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Questionnaire",
                    Description = "Optional description",
                    ActivatedAt = DateTime.UtcNow,
                    Student = new UserBase { UserName = "student1", FullName = "Student One" },
                    Teacher = new UserBase { UserName = "teacher1", FullName = "Teacher One" },
                    StudentCompletedAt = null,
                    TeacherCompletedAt = null
                }
            };

            // Convert ActiveQuestionnaireBase -> ActiveQuestionnaireStudentBase
            var expectedResult = activeBases.Select(a => a.ToActiveQuestionnaireStudentDTO()).ToList();

            _mockUserService.Setup(s => s.GetPendingActiveQuestionnaires(_userId))
                            .ReturnsAsync(activeBases);

            var result = await _controller.GetPendingActiveQuestionnairesForStudent();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsType<List<ActiveQuestionnaireStudentBase>>(okResult.Value);

            Assert.Single(list);
            Assert.Equal("student1", list.First().Student.UserName);
        }

        [Fact]
        public async Task GetPendingActiveQuestionnairesForTeacher_ReturnsOk()
        {
            var activeBases = new List<ActiveQuestionnaireBase>
            {
                new ActiveQuestionnaireBase
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Questionnaire",
                    Description = "Optional description",
                    ActivatedAt = DateTime.UtcNow,
                    Student = new UserBase { UserName = "student1", FullName = "Student One" },
                    Teacher = new UserBase { UserName = "teacher1", FullName = "Teacher One" },
                    StudentCompletedAt = null,
                    TeacherCompletedAt = null
                }
            };

            var expectedResult = activeBases.Select(a => a.ToActiveQuestionnaireTeacherDTO()).ToList();

            _mockUserService.Setup(s => s.GetPendingActiveQuestionnaires(_userId))
                            .ReturnsAsync(activeBases);

            var result = await _controller.GetPendingActiveQuestionnairesForTeacher();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsType<List<ActiveQuestionnaireTeacherBase>>(okResult.Value);

            Assert.Single(list);
            Assert.Equal("teacher1", list.First().Teacher.UserName);
        }
    }
}
