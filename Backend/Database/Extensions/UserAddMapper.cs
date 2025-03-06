using Database.DTO.User;
using Database.Models;

namespace Database.Extensions;

public static class UserAddMapper
{
    public static StudentModel ToStudentModel(this UserAdd student)
    {
        return new()
        {
            Guid = student.Guid,
            FullName = student.FullName,
            UserName = student.UserName,
            PrimaryRole = student.PrimaryRole,
            Permissions = student.Permissions
        };
    }

    public static TeacherModel ToTeacherModel(this UserAdd teacher)
    {
        return new()
        {
            Guid = teacher.Guid,
            FullName = teacher.FullName,
            UserName = teacher.UserName,
            PrimaryRole = teacher.PrimaryRole,
            Permissions = teacher.Permissions
        };
    }
}
