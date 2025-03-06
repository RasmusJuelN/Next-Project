using Database.DTO.User;
using Database.Models;

namespace Database.Extensions;

public static class UserModelMapper
{
    public static UserBase ToBaseDto(this StudentModel student)
    {
        return new()
        {
            UserName = student.UserName,
            FullName = student.FullName
        };
    }

    public static User ToDto(this StudentModel student)
    {
        return new()
        {
            Guid = student.Guid,
            UserName = student.UserName,
            FullName = student.FullName,
            PrimaryRole = student.PrimaryRole,
            Permissions = student.Permissions
        };
    }

    public static UserBase ToBaseDto(this TeacherModel teacher)
    {
        return new()
        {
            UserName = teacher.UserName,
            FullName = teacher.FullName
        };
    }

    public static User ToDto(this TeacherModel teacher)
    {
        return new()
        {
            Guid = teacher.Guid,
            UserName = teacher.UserName,
            FullName = teacher.FullName,
            PrimaryRole = teacher.PrimaryRole,
            Permissions = teacher.Permissions
        };
    }
}
