using API.DTO.User;

namespace UnitTests.Services
{

    public class UserServiceTests
    {
        private readonly Mock<IAuthenticationBridge> _authBridgeMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _authBridgeMock = new Mock<IAuthenticationBridge>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userService = new UserService(_authBridgeMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public void QueryLDAPUsersWithPagination_ShouldReturnMappedUsers()
        {
            // Arrange
            var request = new UserQueryPagination
            {
                User = "alice",
                Role = (Roles)Database.Enums.UserRoles.Student,
                PageSize = 10,
                SessionId = null
            };

            var ldapUsers = new List<BasicUserInfoWithUserID>
            {
                 new BasicUserInfoWithUserID
                {
                    UserId = Guid.NewGuid().ToString(),
                    Name = "Alice Wonderland",
                    Username = "alice123"
                }
            };
            string sessionId = "session1";
            bool hasMore = false;

            _authBridgeMock
                .Setup(a => a.SearchUserPagination<BasicUserInfoWithUserID>(
                    request.User,
                    request.Role.ToString(),
                    request.PageSize,
                    request.SessionId))
                .Returns((ldapUsers, sessionId, hasMore));

            // Act
            var result = _userService.QueryLDAPUsersWithPagination(request);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.UserBases);
            Assert.Equal("Alice Wonderland", result.UserBases.First().FullName);
            Assert.Equal(sessionId, result.SessionId);
            Assert.False(result.HasMore);
        }

        [Fact]
        public async Task GetActiveQuestionnairesForStudent_ShouldReturnPaginatedResults()
        {
            // Arrange
            var request = new ActiveQuestionnaireKeysetPaginationRequestStudent
            {
                PageSize = 5,
                QueryCursor = null,
                Title = "Test",
                FilterStudentCompleted = false,
                Order = ActiveQuestionnaireOrderingOptions.ActivatedAtAsc

            };
            var userId = Guid.NewGuid();
            // Create required Student and Teacher DTOs
            var studentDto = new UserBase
            {

                UserName = "student1",
                FullName = "Student One",
                //PrimaryRole = UserRoles.Student,
                //Permissions = UserPermissions.Read

            };

            var teacherDto = new UserBase
            {

                UserName = "teacher1",
                FullName = "Teacher One"

            };
            var activeQuestionnaires = new List<ActiveQuestionnaireBase>
            {
            new ActiveQuestionnaireBase
            {
                Id = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                QuestionnaireType = ActiveQuestionnaireType.Standard,
                Title = "Test Q1",
                ActivatedAt = DateTime.UtcNow,
                Student = studentDto,
                Teacher = teacherDto,
                TeacherCompletedAt = DateTime.UtcNow,
                StudentCompletedAt = DateTime.UtcNow
            },
            new ActiveQuestionnaireBase
            {
                Id = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                QuestionnaireType = ActiveQuestionnaireType.Standard,
                Title = "Test Q2",
                ActivatedAt = DateTime.UtcNow,
                Student = studentDto,
                Teacher = teacherDto,
                TeacherCompletedAt = DateTime.UtcNow,
                StudentCompletedAt = DateTime.UtcNow
            }
            };

            _unitOfWorkMock
          .Setup(u => u.ActiveQuestionnaire.PaginationQueryWithKeyset(
              It.IsAny<int>(),                  // amount
              It.IsAny<ActiveQuestionnaireOrderingOptions>(), // sortOrder
              It.IsAny<Guid?>(),                // cursorIdPosition
              It.IsAny<DateTime?>(),            // cursorActivatedAtPosition
              It.IsAny<string?>(),              // titleQuery
              It.IsAny<string?>(),              // student
              It.IsAny<string?>(),              // teacher
              It.IsAny<Guid?>(),                // idQuery
              It.IsAny<Guid?>(),                // userId
              It.IsAny<bool>(),                 // onlyStudentCompleted
              It.IsAny<bool>(),                 // onlyTeacherCompleted
              It.IsAny<bool>(),                 // pendingStudent
              It.IsAny<bool>(),                 // pendingTeacher
              It.IsAny<ActiveQuestionnaireType>()  // questionnaireType
          ))
          .ReturnsAsync((activeQuestionnaires, activeQuestionnaires.Count));

            // Act
            var result = await _userService.GetActiveQuestionnairesForStudent(request, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.ActiveQuestionnaireBases.Count);
            Assert.Equal(activeQuestionnaires.Count, result.TotalCount);
            Assert.NotNull(result.QueryCursor);
            Assert.Equal("Test Q1", result.ActiveQuestionnaireBases.First().Title);
            Assert.Equal("Student One", result.ActiveQuestionnaireBases.First().Student.FullName);
            //Assert.Equal("Teacher One", result.ActiveQuestionnaireBases.First().Teacher.FullName);
        }

        [Fact]
        public async Task GetPendingActiveQuestionnaires_ShouldReturnPendingList()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var dummyStudent = new UserBase
            {
                UserName = "student1",
                FullName = "Student One"
            };

            var dummyTeacher = new UserBase
            {
                UserName = "teacher1",
                FullName = "Teacher One"
            };

            var pendingList = new List<ActiveQuestionnaireBase>
    {
        new ActiveQuestionnaireBase
        {
            Id = Guid.NewGuid(),
            GroupId = Guid.NewGuid(),
            QuestionnaireType = ActiveQuestionnaireType.Standard,
            Title = "Pending Q1",
            ActivatedAt = DateTime.UtcNow,
            Student = dummyStudent,
            Teacher = dummyTeacher,
            StudentCompletedAt = null,
            TeacherCompletedAt = null
        }
    };

            _unitOfWorkMock
                .Setup(u => u.ActiveQuestionnaire.GetPendingActiveQuestionnaires(userId))
                .ReturnsAsync(pendingList);

            // Act
            var result = await _userService.GetPendingActiveQuestionnaires(userId);

            // Assert
            Assert.Single(result);
            Assert.Equal("Pending Q1", result.First().Title);
        }


        //[Fact]
        //public async Task GetPendingActiveQuestionnaires_ShouldReturnPendingList()
        //{
        //    // Arrange
        //    var userId = Guid.NewGuid();
        //    var pendingList = new List<ActiveQuestionnaireBase>
        //    {
        //         new ActiveQuestionnaireBase
        //        {
        //            Id = Guid.NewGuid(),
        //            Title = "Pending Q1",
        //            Teacher = null!,
        //            Student = null!,
        //            ActivatedAt = DateTime.UtcNow
        //        }
        //    };

        //    _unitOfWorkMock
        //        .Setup(u => u.ActiveQuestionnaire.GetPendingActiveQuestionnaires(userId))
        //        .ReturnsAsync(pendingList);

        //    // Act
        //    var result = await _userService.GetPendingActiveQuestionnaires(userId);

        //    // Assert
        //    Assert.Single(result);
        //    Assert.Equal("Pending Q1", result.First().Title);
        //}

    }

}
