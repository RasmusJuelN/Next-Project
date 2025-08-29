using Database.DTO.User;
using Database.Models;

namespace Database.Extensions;

/// <summary>
/// Provides extension methods for mapping user-specific database models (Student/Teacher) to DTOs.
/// This mapper handles conversions from Entity Framework user models to response DTOs for API endpoints.
/// </summary>
public static class UserModelMapper
{
    /// <summary>
    /// Converts a StudentModel to a UserBase DTO containing essential user information.
    /// </summary>
    /// <param name="student">The StudentModel from the database.</param>
    /// <returns>A UserBase DTO with basic student identification information.</returns>
    /// <remarks>
    /// This method creates a lightweight DTO containing only essential student identification details.
    /// Useful for scenarios where minimal user information is required without exposing
    /// security-sensitive details like roles or permissions.
    /// </remarks>
    public static UserBase ToBaseDto(this StudentModel student)
    {
        return new()
        {
            UserName = student.UserName,
            FullName = student.FullName
        };
    }

    /// <summary>
    /// Converts a StudentModel to a FullUser DTO containing complete user information.
    /// </summary>
    /// <param name="student">The StudentModel from the database.</param>
    /// <returns>A FullUser DTO with comprehensive student details including LDAP information, roles, and permissions.</returns>
    /// <remarks>
    /// This method creates a comprehensive DTO containing all student information including security-sensitive
    /// details. Should only be used in contexts where the requesting user has appropriate authorization
    /// to view detailed student information.
    /// </remarks>
    public static FullUser ToDto(this StudentModel student)
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

    /// <summary>
    /// Converts a TeacherModel to a UserBase DTO containing essential user information.
    /// </summary>
    /// <param name="teacher">The TeacherModel from the database.</param>
    /// <returns>A UserBase DTO with basic teacher identification information.</returns>
    /// <remarks>
    /// This method creates a lightweight DTO containing only essential teacher identification details.
    /// Useful for scenarios where minimal user information is required without exposing
    /// security-sensitive details like roles or permissions.
    /// </remarks>
    public static UserBase ToBaseDto(this TeacherModel teacher)
    {
        return new()
        {
            UserName = teacher.UserName,
            FullName = teacher.FullName
        };
    }

    /// <summary>
    /// Converts a TeacherModel to a FullUser DTO containing complete user information.
    /// </summary>
    /// <param name="teacher">The TeacherModel from the database.</param>
    /// <returns>A FullUser DTO with comprehensive teacher details including LDAP information, roles, and permissions.</returns>
    /// <remarks>
    /// This method creates a comprehensive DTO containing all teacher information including security-sensitive
    /// details. Should only be used in contexts where the requesting user has appropriate authorization
    /// to view detailed teacher information.
    /// </remarks>
    public static FullUser ToDto(this TeacherModel teacher)
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
