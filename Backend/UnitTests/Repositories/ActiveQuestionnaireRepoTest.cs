
namespace UnitTests.Repos
{
    public class ActiveQuestionnaireRepoTest 
    {
        private readonly DbContextOptions<Context> _options;
        private readonly Context _context;
        private readonly ActiveQuestionnaireRepository _repo;
        public ActiveQuestionnaireRepoTest()
        {
            _options = new DbContextOptionsBuilder<Context>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            _context = new Context(_options);
            _repo = new ActiveQuestionnaireRepository(_context, NullLoggerFactory.Instance);
        }
        [Fact]
        public async Task ActivateQuestionnaireAsync_ShouldCreateActiveQuestionnaire()
        {
            // Arrange: create Student, Teacher, Template
            var student = new StudentModel
            {
                Guid = Guid.NewGuid(),
                UserName = "student1",
                FullName = "Student One",
                PrimaryRole = UserRoles.Student,
                Permissions = UserPermissions.Student
            };

            var teacher = new TeacherModel
            {
                Guid = Guid.NewGuid(),
                UserName = "teacher1",
                FullName = "Teacher One",
                PrimaryRole = UserRoles.Teacher,
                Permissions = UserPermissions.Teacher
            };

            var template = new QuestionnaireTemplateModel
            {
                Title = "Template 1",
                Description = "Test Template",
                Questions = new List<QuestionnaireQuestionModel>()
            };

            // Save users and template
            _context.Users.AddRange(student, teacher);
            _context.QuestionnaireTemplates.Add(template);
            await _context.SaveChangesAsync();



            var groupId = Guid.NewGuid();

            // Act
            var activeQuestionnaire = await _repo.ActivateQuestionnaireAsync(
                questionnaireTemplateId: template.Id,
                studentId: student.Guid,
                teacherId: teacher.Guid,
                groupId: groupId
            );
            // 
            var unitOfWork = new UnitOfWork(_context, null, _repo, null, null, null); // inject other repos as needed
            await unitOfWork.SaveChangesAsync(); // persist the entity

            // Assert DTO mapping
            Assert.NotNull(activeQuestionnaire);
            Assert.Equal(template.Title, activeQuestionnaire.Title);
            Assert.Equal(student.UserName, activeQuestionnaire.Student.UserName);
            Assert.Equal(student.FullName, activeQuestionnaire.Student.FullName);
            Assert.Equal(teacher.UserName, activeQuestionnaire.Teacher.UserName);
            Assert.Equal(teacher.FullName, activeQuestionnaire.Teacher.FullName);



            // Assert persistence in database
            var dbCheck = await _context.ActiveQuestionnaires
                .Include(a => a.Student)
                .Include(a => a.Teacher)
                .Include(a => a.QuestionnaireTemplate)
                .FirstOrDefaultAsync(a => a.Id == activeQuestionnaire.Id);

            Assert.NotNull(dbCheck);
            Assert.Equal(student.Guid, dbCheck.Student.Guid);
            Assert.Equal(teacher.Guid, dbCheck.Teacher.Guid);
            Assert.Equal(template.Id, dbCheck.QuestionnaireTemplate.Id);
            Assert.Equal(groupId, dbCheck.GroupId);
        }

        [Fact]
        public async Task GetActiveQuestionnaireBaseAsync_ShouldReturnBaseDto()
        {
            // Arrange
            var student = new StudentModel
            {
                Guid = Guid.NewGuid(),
                UserName = "s1",
                FullName = "S1",
                PrimaryRole = UserRoles.Student,
                Permissions = UserPermissions.Student
            };
            var teacher = new TeacherModel
            {
                Guid = Guid.NewGuid(),
                UserName = "t1",
                FullName = "T1",
                PrimaryRole = UserRoles.Teacher,
                Permissions = UserPermissions.Teacher
            };
            var template = new QuestionnaireTemplateModel
            {
                Title = "Template 1",
                Description = "Test Template",
                Questions = new List<QuestionnaireQuestionModel>()

            };
            var activeQ = new ActiveQuestionnaireModel
            {
                Title = "Q1",
                Student = student,
                Teacher = teacher,
                QuestionnaireTemplate = template,
                ActivatedAt = DateTime.UtcNow
            };

            _context.Users.AddRange(student, teacher);
            _context.ActiveQuestionnaires.Add(activeQ);
            _context.ActiveQuestionnaires.Add(activeQ);

            await _context.SaveChangesAsync();

            // Act
            var result = await _repo.GetActiveQuestionnaireBaseAsync(activeQ.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(activeQ.Title, result.Title);
            Assert.Equal(student.UserName, result.Student.UserName);
            Assert.Equal(teacher.UserName, result.Teacher.UserName);



        }

        [Fact]
        public async Task HasUserSubmittedAnswer_ShouldReturnCorrectly()
        {
            // Arrange
            var student = new StudentModel { Guid = Guid.NewGuid(), UserName = "s1", FullName = "S1", PrimaryRole = UserRoles.Student, Permissions = UserPermissions.Student };
            var teacher = new TeacherModel { Guid = Guid.NewGuid(), UserName = "t1", FullName = "T1", PrimaryRole = UserRoles.Teacher, Permissions = UserPermissions.Teacher };
            var template = new QuestionnaireTemplateModel
            {

                Title = "Template 1",
                Description = "Test Template",
                Questions = new List<QuestionnaireQuestionModel>()
            };
            var activeQ = new ActiveQuestionnaireModel
            {

                Title = "Q1",
                Student = student,
                Teacher = teacher,
                QuestionnaireTemplate = template,
                ActivatedAt = DateTime.UtcNow,
                StudentCompletedAt = DateTime.UtcNow // mark student as completed
            };
            _context.Users.AddRange(student, teacher);
            _context.ActiveQuestionnaires.Add(activeQ);
            await _context.SaveChangesAsync();

            // Act & Assert
            bool studentSubmitted = await _repo.HasUserSubmittedAnswer(student.Guid, activeQ.Id);
            bool teacherSubmitted = await _repo.HasUserSubmittedAnswer(teacher.Guid, activeQ.Id);
            Assert.True(await _repo.HasUserSubmittedAnswer(student.Guid, activeQ.Id));
            Assert.False(await _repo.HasUserSubmittedAnswer(teacher.Guid, activeQ.Id));
        }
    }
}
