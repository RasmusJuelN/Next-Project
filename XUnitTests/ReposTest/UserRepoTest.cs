using Database;
using Database.DTO.ActiveQuestionnaire;
using Database.DTO.User;
using Database.Enums;
using Database.Models;
using Database.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTests.ReposTest
{
    public class UserRepoTest
    {
        private readonly DbContextOptions<Context> _options;
        private readonly Context _context;
        private readonly UserRepository _userRepository;
        public UserRepoTest()            {
            _options = new DbContextOptionsBuilder<Context>()
               .UseInMemoryDatabase(databaseName: "UserTestDb") // unique DB name per test class
               .Options;

            _context = new Context(_options);
            _userRepository = new UserRepository(_context, NullLoggerFactory.Instance);
        }
        #region GetUser
        [Fact]
        public async void GetUserAsync_ShouldReturnUser_WhenUserExists()
        {
            await _context.Database.EnsureDeletedAsync();

            var student = new StudentModel
            {
                Guid = Guid.NewGuid(),
                UserName = "alice123",
                FullName = "Alice Wonderland",
                PrimaryRole = UserRoles.Student,          
                Permissions = UserPermissions.Student
                                        
            };

            _context.Users.Add(student);
            await _context.SaveChangesAsync();

            var result = await _userRepository.GetUserAsync(student.Guid);

            Assert.NotNull(result);
            Assert.Equal(student.Guid, result?.Guid);
            Assert.Equal("Alice Wonderland", result?.FullName);
            Assert.Equal(UserPermissions.Student, result?.Permissions);
            Assert.Equal(UserRoles.Student, UserRoles.Student);
        }

        [Fact]
        public async void GetUserAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            await _context.Database.EnsureDeletedAsync();

            var result = await _userRepository.GetUserAsync(Guid.NewGuid());
            Assert.Null(result);
        }
        #endregion

        #region GetStudent
        [Fact]
        public async void GetStudentAsync_ShouldReturnStudent_WhenStudentExists()
        {
            await _context.Database.EnsureDeletedAsync();

            var student = new StudentModel
            {
                Guid = Guid.NewGuid(),
                UserName = "alice123",
                FullName = "Alice Wonderland",
                PrimaryRole = UserRoles.Student,
                Permissions = UserPermissions.Student

            };

            _context.Users.Add(student);
            await _context.SaveChangesAsync();

            var result = await _userRepository.GetStudentAsync(student.Guid);

            Assert.NotNull(result);
            Assert.Equal(student.Guid, result?.Guid);
            Assert.Equal("Alice Wonderland", result?.FullName);
        }

        [Fact]
        public async void GetStudentAsync_ShouldReturnNull_WhenStudentDoesNotExist()
        {
            await _context.Database.EnsureDeletedAsync();

            var result = await _userRepository.GetStudentAsync(Guid.NewGuid());
            Assert.Null(result);
        }
        #endregion

        #region GetTeacher
        [Fact]
        public async void GetTeacherAsync_ShouldReturnTeacher_WhenTeacherExists()
        {
            await _context.Database.EnsureDeletedAsync();

            var teacher = new TeacherModel 
            { 
                Guid = Guid.NewGuid(),
                UserName= "smith456",
                FullName = "Smith Joshi",
                PrimaryRole = UserRoles.Teacher,
                Permissions = UserPermissions.Teacher,

            };
            _context.Users.Add(teacher);
            await _context.SaveChangesAsync();

            var result = await _userRepository.GetTeacherAsync(teacher.Guid);

            Assert.NotNull(result);
            Assert.Equal(teacher.Guid, result?.Guid);
            Assert.Equal("Smith Joshi", result?.FullName);
        }

        [Fact]
        public async void GetTeacherAsync_ShouldReturnNull_WhenTeacherDoesNotExist()
        {
            await _context.Database.EnsureDeletedAsync();

            var result = await _userRepository.GetTeacherAsync(Guid.NewGuid());
            Assert.Null(result);
        }
        #endregion

        #region AddStudent
        [Fact]
        public async void AddStudentAsync_ShouldAddStudent_WhenValidStudentProvided()
        {
            await _context.Database.EnsureDeletedAsync();

            var userAdd = new UserAdd
            {
              Guid = Guid.NewGuid(),
              FullName = "Ram Shah",
                UserName = "ram.shah",
              PrimaryRole = UserRoles.Student,
              Permissions = UserPermissions.Student
             

            };

            await _userRepository.AddStudentAsync(userAdd);
            await _context.SaveChangesAsync();

            var student = await _context.Users.OfType<StudentModel>()
                .FirstOrDefaultAsync(u => u.Guid == userAdd.Guid);

            Assert.NotNull(student);
            Assert.Equal("Ram Shah", student?.FullName);  
            Assert.Equal("ram.shah", student?.UserName);
            Assert.Equal(UserRoles.Student, student?.PrimaryRole);
            Assert.Equal(UserPermissions.Student, student?.Permissions);
        }
        #endregion

        #region AddTeacher
        [Fact]
        public async Task AddTeacherAsync_ShouldAddTeacher_WhenValidTeacherProvided()
        {
            await _context.Database.EnsureDeletedAsync();

            var userAdd = new UserAdd
            {
                Guid = Guid.NewGuid(),
                FullName = "Sita Devi",
                UserName = "sita.devi",
                PrimaryRole = UserRoles.Teacher,
                Permissions = UserPermissions.Teacher
            };

            await _userRepository.AddTeacherAsync(userAdd);
            await _context.SaveChangesAsync();

            var teacher = await _context.Users.OfType<TeacherModel>()
                .FirstOrDefaultAsync(u => u.Guid == userAdd.Guid);

            Assert.NotNull(teacher);
            Assert.Equal("Sita Devi", teacher?.FullName);
            Assert.Equal("sita.devi", teacher?.UserName);
            Assert.Equal(UserRoles.Teacher, teacher?.PrimaryRole);
            Assert.Equal(UserPermissions.Teacher, teacher?.Permissions);
        }
        #endregion

        #region UserExists
        [Fact]
        public async void UserExists_ShouldReturnTrue_WhenUserExists()
        {
            await _context.Database.EnsureDeletedAsync();

            var teacher = new TeacherModel
            {
                Guid = Guid.NewGuid(),
                FullName = "Jane Abc",
                PrimaryRole = UserRoles.Teacher,
                Permissions = UserPermissions.Teacher,
                UserName = "Jane12"
            };
            _context.Users.Add(teacher);
            await _context.SaveChangesAsync();

            var result = _userRepository.UserExists(teacher.Guid);
            Assert.True(result);
        }

        [Fact]
        public async void UserExists_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            await _context.Database.EnsureDeletedAsync();

            var result = _userRepository.UserExists(Guid.NewGuid());
            Assert.False(result);
        }
        #endregion
    }
}
