//using Database;
//using Database.DTO.User;
//using Database.Models;
//using Database.Repository;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging.Abstractions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;

//namespace UnitTests.RepoTest
//{
//    public class UserRepoTest
//    {
//        private readonly DbContextOptions<Context> _options;
//        private readonly Context _context;
//        private readonly UserRepository _userRepository;

//        public UserRepoTest()
//        {
//            _options = new DbContextOptionsBuilder<Context>()
//                .UseInMemoryDatabase(databaseName: "UserTestDb") // unique DB name per test class
//                .Options;

//            _context = new Context(_options);
//            _userRepository = new UserRepository(_context, NullLoggerFactory.Instance);
//        }

//        #region GetUser
//        [Fact]
//        public async void GetUserAsync_ShouldReturnUser_WhenUserExists()
//        {
//            await _context.Database.EnsureDeletedAsync();

//            var student = new StudentModel
//            {
//                Guid = Guid.NewGuid(), 
//                UserName = "Alice123",
//                FullName = "Alice Wonderland"  // <-- required
//            };

//            _context.Users.Add(student);
//            await _context.SaveChangesAsync();

//            var result = await _userRepository.GetUserAsync(student.Guid);

//            Assert.NotNull(result);
//            Assert.Equal(student.Guid, result?.Guid);
//            Assert.Equal("Alice", result?.Name);
//        }

//        [Fact]
//        public async void GetUserAsync_ShouldReturnNull_WhenUserDoesNotExist()
//        {
//            await _context.Database.EnsureDeletedAsync();

//            var result = await _userRepository.GetUserAsync(Guid.NewGuid());
//            Assert.Null(result);
//        }
//        #endregion

//        #region GetStudent
//        [Fact]
//        public async void GetStudentAsync_ShouldReturnStudent_WhenStudentExists()
//        {
//            await _context.Database.EnsureDeletedAsync();

//            var student = new StudentModel { Guid = Guid.NewGuid(), Name = "Bob" };
//            _context.Users.Add(student);
//            await _context.SaveChangesAsync();

//            var result = await _userRepository.GetStudentAsync(student.Guid);

//            Assert.NotNull(result);
//            Assert.Equal(student.Guid, result?.Guid);
//            Assert.Equal("Bob", result?.Name);
//        }

//        [Fact]
//        public async void GetStudentAsync_ShouldReturnNull_WhenStudentDoesNotExist()
//        {
//            await _context.Database.EnsureDeletedAsync();

//            var result = await _userRepository.GetStudentAsync(Guid.NewGuid());
//            Assert.Null(result);
//        }
//        #endregion

//        #region GetTeacher
//        [Fact]
//        public async void GetTeacherAsync_ShouldReturnTeacher_WhenTeacherExists()
//        {
//            await _context.Database.EnsureDeletedAsync();

//            var teacher = new TeacherModel { Guid = Guid.NewGuid(), Name = "Mr. Smith" };
//            _context.Users.Add(teacher);
//            await _context.SaveChangesAsync();

//            var result = await _userRepository.GetTeacherAsync(teacher.Guid);

//            Assert.NotNull(result);
//            Assert.Equal(teacher.Guid, result?.Guid);
//            Assert.Equal("Mr. Smith", result?.Name);
//        }

//        [Fact]
//        public async void GetTeacherAsync_ShouldReturnNull_WhenTeacherDoesNotExist()
//        {
//            await _context.Database.EnsureDeletedAsync();

//            var result = await _userRepository.GetTeacherAsync(Guid.NewGuid());
//            Assert.Null(result);
//        }
//        #endregion

//        #region AddStudent
//        [Fact]
//        public async void AddStudentAsync_ShouldAddStudent_WhenValidStudentProvided()
//        {
//            await _context.Database.EnsureDeletedAsync();

//            var userAdd = new UserAdd { Guid = Guid.NewGuid(), Name = "Student X" };

//            await _userRepository.AddStudentAsync(userAdd);

//            var student = await _context.Users.OfType<StudentModel>()
//                .FirstOrDefaultAsync(u => u.Guid == userAdd.Guid);

//            Assert.NotNull(student);
//            Assert.Equal("Student X", student?.Name);
//        }
//        #endregion

//        #region AddTeacher
//        [Fact]
//        public async void AddTeacherAsync_ShouldAddTeacher_WhenValidTeacherProvided()
//        {
//            await _context.Database.EnsureDeletedAsync();

//            var userAdd = new UserAdd { Guid = Guid.NewGuid(), Name = "Teacher Y" };

//            await _userRepository.AddTeacherAsync(userAdd);

//            var teacher = await _context.Users.OfType<TeacherModel>()
//                .FirstOrDefaultAsync(u => u.Guid == userAdd.Guid);

//            Assert.NotNull(teacher);
//            Assert.Equal("Teacher Y", teacher?.Name);
//        }
//        #endregion

//        #region UserExists
//        [Fact]
//        public async void UserExists_ShouldReturnTrue_WhenUserExists()
//        {
//            await _context.Database.EnsureDeletedAsync();

//            var teacher = new TeacherModel { Guid = Guid.NewGuid(), Name = "Ms. Jane" };
//            _context.Users.Add(teacher);
//            await _context.SaveChangesAsync();

//            var result = _userRepository.UserExists(teacher.Guid);
//            Assert.True(result);
//        }

//        [Fact]
//        public async void UserExists_ShouldReturnFalse_WhenUserDoesNotExist()
//        {
//            await _context.Database.EnsureDeletedAsync();

//            var result = _userRepository.UserExists(Guid.NewGuid());
//            Assert.False(result);
//        }
//        #endregion
//    }
//}
