using Database.DTO.User;

namespace Database.Interfaces;

/// <summary>
/// Defines the contract for user repository operations.
/// Manages user data retrieval, existence checking, and user creation for both students and teachers.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves complete information about a student by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <returns>A FullUser DTO containing complete student information, or null if not found.</returns>
    /// <remarks>
    /// This method specifically queries the student table and returns comprehensive user data
    /// including roles, permissions, and additional information. Should only be used when student-specific
    /// data access is required.
    /// </remarks>
    Task<FullUser?> GetStudentAsync(Guid id);

    /// <summary>
    /// Retrieves complete information about a teacher by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the teacher.</param>
    /// <returns>A FullUser DTO containing complete teacher information, or null if not found.</returns>
    /// <remarks>
    /// This method specifically queries the teacher table and returns comprehensive user data
    /// including roles, permissions, and additional information. Should only be used when teacher-specific
    /// data access is required.
    /// </remarks>
    Task<FullUser?> GetTeacherAsync(Guid id);

    /// <summary>
    /// Retrieves complete information about a user regardless of their role (student or teacher).
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the user.</param>
    /// <returns>A FullUser DTO containing complete user information, or null if not found.</returns>
    /// <remarks>
    /// This method searches across both student and teacher tables to locate the user.
    /// Use this method when the user's role is unknown or when role-agnostic user lookup is needed.
    /// </remarks>
    Task<FullUser?> GetUserAsync(Guid id);

    /// <summary>
    /// Retrieves the ID of the oldest active questionnaire assigned to a specific user.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The GUID of the oldest active questionnaire, or null if no active questionnaires exist.</returns>
    /// <remarks>
    /// This method helps prioritize user workflow by identifying the longest-pending questionnaire.
    /// Useful for dashboard displays and task management where oldest tasks should be highlighted.
    /// </remarks>
    Task<Guid?> GetIdOfOldestActiveQuestionnaire(Guid id);
    /// <summary>
    /// Checks if a user exists in the system by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the user to check.</param>
    /// <returns>True if the user exists in either student or teacher tables, false otherwise.</returns>
    /// <remarks>
    /// This method provides an efficient existence check without loading user data.
    /// Searches across both student and teacher tables for comprehensive user existence validation.
    /// </remarks>
    bool UserExists(Guid id);

    /// <summary>
    /// Checks if a user exists in the system by their primary key.
    /// </summary>
    /// <param name="primaryKey">The integer primary key of the user to check.</param>
    /// <returns>True if the user exists with the specified primary key, false otherwise.</returns>
    /// <remarks>
    /// This method provides existence checking using the database's internal primary key.
    /// Typically used for internal database operations where primary keys are available.
    /// </remarks>
    bool UserExists(int primaryKey);

    /// <summary>
    /// Creates a new student record in the database.
    /// </summary>
    /// <param name="student">The UserAdd DTO containing student creation data.</param>
    /// <exception cref="ArgumentException">Thrown when the student data is invalid or incomplete.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a user with the same GUID already exists.</exception>
    /// <remarks>
    /// This method creates a new student record with the provided user and role information.
    /// The GUID should typically come from an external identity provider like Active Directory.
    /// </remarks>
    Task AddStudentAsync(UserAdd student);

    /// <summary>
    /// Creates a new teacher record in the database.
    /// </summary>
    /// <param name="teacher">The UserAdd DTO containing teacher creation data.</param>
    /// <exception cref="ArgumentException">Thrown when the teacher data is invalid or incomplete.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a user with the same GUID already exists.</exception>
    /// <remarks>
    /// This method creates a new teacher record with the provided user and role information.
    /// The GUID should typically come from an external identity provider like Active Directory.
    /// </remarks>
    Task AddTeacherAsync(UserAdd teacher);

    Task<int?> GetIdByGuidAsync(Guid guid);
}
