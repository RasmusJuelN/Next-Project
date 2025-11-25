
namespace UnitTests.Repositories
{
    public class GenericRepositoryTests
    {
        private readonly DbContextOptions<Context> _options;
        private readonly Context _context;
        private readonly GenericRepository <StudentModel> _repo;
        public GenericRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<Context>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            _context = new Context(_options);
            _repo = new GenericRepository<StudentModel>(_context, NullLoggerFactory.Instance);
        }
        [Fact]
        public async Task AddAsync_ShouldAddStudent()
        {
            // Arrange
            var student = new StudentModel
            {
                Guid = Guid.NewGuid(),
                UserName = "student1",
                FullName = "Student One",
                PrimaryRole = Database.Enums.UserRoles.Student,
                Permissions = Database.Enums.UserPermissions.Student
            };

            // Act
            await _repo.AddAsync(student);
            await _context.SaveChangesAsync();

            // Assert
            var savedStudent = await _context.Users.OfType<StudentModel>().FirstOrDefaultAsync();
            Assert.NotNull(savedStudent);
            Assert.Equal(student.Guid, savedStudent.Guid);
        }

        [Fact]
        public async Task AddRangeAsync_ShouldAddMultipleStudents()
        {
            // Arrange
            var students = new List<StudentModel>
            {
                new StudentModel { Guid = Guid.NewGuid(), UserName = "s1", FullName = "S1", PrimaryRole = Database.Enums.UserRoles.Student, Permissions = Database.Enums.UserPermissions.Student },
                new StudentModel { Guid = Guid.NewGuid(), UserName = "s2", FullName = "S2", PrimaryRole = Database.Enums.UserRoles.Student, Permissions = Database.Enums.UserPermissions.Student }
            };

            // Act
            await _repo.AddRangeAsync(students);
            await _context.SaveChangesAsync();

            // Assert
            var savedStudents = await _context.Users.OfType<StudentModel>().ToListAsync();
            Assert.Equal(2, savedStudents.Count);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnMatchingStudents()
        {
            // Arrange
            var student1 = new StudentModel { Guid = Guid.NewGuid(), UserName = "s1", FullName = "S1", PrimaryRole = Database.Enums.UserRoles.Student, Permissions = Database.Enums.UserPermissions.Student };
            var student2 = new StudentModel { Guid = Guid.NewGuid(), UserName = "s2", FullName = "S2", PrimaryRole = Database.Enums.UserRoles.Student, Permissions = Database.Enums.UserPermissions.Student };
            _context.Users.AddRange(student1, student2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repo.GetAsync(s => s.UserName == "s1");

            // Assert
            Assert.Single(result);
            Assert.Equal("s1", result[0].UserName);
        }

        [Fact]
        public async Task GetSingleAsync_ShouldReturnSingleStudent()
        {
            // Arrange
            var student = new StudentModel { Guid = Guid.NewGuid(), UserName = "s1", FullName = "S1", PrimaryRole = Database.Enums.UserRoles.Student, Permissions = Database.Enums.UserPermissions.Student };
            _context.Users.Add(student);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repo.GetSingleAsync(s => s.UserName == "s1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("s1", result.UserName);
        }

        [Fact]
        public async Task CountAndExists_ShouldWorkCorrectly()
        {
            // Arrange
            var student = new StudentModel { Guid = Guid.NewGuid(), UserName = "s1", FullName = "S1", PrimaryRole = Database.Enums.UserRoles.Student, Permissions = Database.Enums.UserPermissions.Student };
            _context.Users.Add(student);
            await _context.SaveChangesAsync();

            // Act & Assert
            Assert.Equal(1, _repo.Count());
            Assert.True(_repo.Exists(s => s.UserName == "s1"));
            Assert.False(_repo.Exists(s => s.UserName == "notfound"));
        }

        [Fact]
        public async Task Delete_ShouldRemoveStudent()
        {
            // Arrange
            var student = new StudentModel { Guid = Guid.NewGuid(), UserName = "s1", FullName = "S1", PrimaryRole = Database.Enums.UserRoles.Student, Permissions = Database.Enums.UserPermissions.Student };
            _context.Users.Add(student);
            await _context.SaveChangesAsync();

            // Act
            _repo.Delete(student);
            await _context.SaveChangesAsync();

            // Assert
            Assert.Empty(await _context.Users.OfType<StudentModel>().ToListAsync());
        }

    }
}
