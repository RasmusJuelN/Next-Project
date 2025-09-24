
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ServicesTest
{
    public class UserServiceTest
    {
        public UserServiceTest()  
        { 

        }
        //[Fact]
        //public void GetStudentsInGroup_ShouldReturnStudents()
        //{
        //    // Arrange
        //    var mockLdapService = new Mock<LdapService>();
        //    var mockUnitOfWork = new Mock<IUnitOfWork>();

        //    var testGroup = "H1";
        //    var fakeStudents = new List<LdapUserDTO>
        //{
        //    new() { Name = "Alice", ClassName = "H1" },
        //    new() { Name = "Bob", ClassName = "H1" }
        //};

        //    mockLdapService.Setup(s => s.GetStudentsInGroup(testGroup))
        //                   .Returns(fakeStudents);

        //    var service = new UserService(mockLdapService.Object, mockUnitOfWork.Object);

        //    // Act
        //    var students = service.GetStudentsInGroup(testGroup);

        //    // Assert
        //    Assert.Equal(2, students.Count);
        //    Assert.Contains(students, s => s.Name == "Alice");
        //    Assert.Contains(students, s => s.Name == "Bob");
        //}

        //[Fact]
        //public void GetStudentsGrouped_ShouldGroupByClass()
        //{
        //    // Arrange
        //    var mockLdapService = new Mock<LdapService>();
        //    var mockUnitOfWork = new Mock<IUnitOfWork>();

        //    var students = new List<LdapUserDTO>
        //{
        //    new() { Name = "Alice", ClassName = "H1" },
        //    new() { Name = "Bob", ClassName = "H1" },
        //    new() { Name = "Charlie", ClassName = "H2" }
        //};

        //    var service = new UserService(mockLdapService.Object, mockUnitOfWork.Object);

        //    // Act
        //    var grouped = service.GetStudentsGrouped(students);

        //    // Assert
        //    Assert.Equal(2, grouped.Count);
        //    Assert.Contains(grouped, g => g.ClassName == "H1" && g.Students.Count == 2);
        //    Assert.Contains(grouped, g => g.ClassName == "H2" && g.Students.Count == 1);
        //}
    }
}
