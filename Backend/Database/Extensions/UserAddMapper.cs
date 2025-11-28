
namespace Database.Extensions;

/// <summary>
/// Provides extension methods for mapping user creation DTOs to database models.
/// This mapper handles conversions from user add request DTOs to Entity Framework models for database persistence.
/// </summary>
public static class UserAddMapper
{
    /// <summary>
    /// Converts a UserAdd DTO to a StudentModel for database persistence.
    /// </summary>
    /// <param name="student">The UserAdd DTO containing student creation data.</param>
    /// <returns>A StudentModel ready for database insertion.</returns>
    /// <remarks>
    /// This method maps user properties to a student-specific model, preserving user identity information,
    /// role assignments, and permission settings. The created model will have system-generated values
    /// for ID and timestamp fields upon database insertion.
    /// </remarks>
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

    /// <summary>
    /// Converts a UserAdd DTO to a TeacherModel for database persistence.
    /// </summary>
    /// <param name="teacher">The UserAdd DTO containing teacher creation data.</param>
    /// <returns>A TeacherModel ready for database insertion.</returns>
    /// <remarks>
    /// This method maps user properties to a teacher-specific model, preserving user identity information,
    /// role assignments, and permission settings. The created model will have system-generated values
    /// for ID and timestamp fields upon database insertion.
    /// </remarks>
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
