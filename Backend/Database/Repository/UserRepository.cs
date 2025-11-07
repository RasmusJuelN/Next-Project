using Database.DTO.User;
using Database.Extensions;
using Database.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Repository;

/// <summary>
/// Implements repository operations for user management.
/// Provides functionality for user data retrieval, existence checking, and user creation for both students and teachers with role-specific operations.
/// </summary>
/// <remarks>
/// This repository manages user operations across the inheritance hierarchy, handling both base user functionality
/// and role-specific operations for students and teachers. It provides efficient user lookup operations and
/// supports the application's workflow management by tracking pending questionnaires and user relationships.
/// </remarks>
/// <param name="context">The database context for data access operations.</param>
/// <param name="loggerFactory">Factory for creating loggers for diagnostic and monitoring purposes.</param>
public class UserRepository(Context context, ILoggerFactory loggerFactory) : IUserRepository
{
    private readonly Context _context = context;
    private readonly GenericRepository<UserBaseModel> _genericRepository = new(context, loggerFactory);

    /// <summary>
    /// Retrieves complete information about a student by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the student.</param>
    /// <returns>A FullUser DTO containing complete student information, or null if not found.</returns>
    /// <remarks>
    /// This method specifically queries the student table and returns comprehensive user data
    /// including roles, permissions, and additional information. Uses the derived StudentModel type
    /// to ensure type safety and role-specific data access.
    /// </remarks>
    public async Task<FullUser?> GetStudentAsync(Guid id)
    {
        StudentModel? student = await _context.Users.OfType<StudentModel>().FirstOrDefaultAsync(t => t.Guid == id);
        return student?.ToDto();
    }

    /// <summary>
    /// Retrieves complete information about a teacher by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the teacher.</param>
    /// <returns>A FullUser DTO containing complete teacher information, or null if not found.</returns>
    /// <remarks>
    /// This method specifically queries the teacher table and returns comprehensive user data
    /// including roles, permissions, and additional information. Uses the derived TeacherModel type
    /// to ensure type safety and role-specific data access.
    /// </remarks>
    public async Task<FullUser?> GetTeacherAsync(Guid id)
    {
        TeacherModel? teacher = await _context.Users.OfType<TeacherModel>().FirstOrDefaultAsync(u => u.Guid == id);
        return teacher?.ToDto();
    }

    /// <summary>
    /// Retrieves complete information about a user, regardless of role type.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the user.</param>
    /// <returns>A FullUser DTO containing user information, or null if not found.</returns>
    /// <remarks>
    /// This method provides a unified interface for retrieving user information across
    /// both student and teacher types. Queries the base UserModel table to retrieve
    /// users regardless of their derived type, making it suitable for scenarios where
    /// role-specific functionality is not required.
    /// </remarks>
    public async Task<FullUser?> GetUserAsync(Guid id)
    {
        UserBaseModel? user = await _context.Users.FirstOrDefaultAsync(u => u.Guid == id);
        return user?.ToDto();
    }

    /// <summary>
    /// Retrieves the identifier of the oldest pending active questionnaire for a specific user.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the user (student or teacher).</param>
    /// <returns>The GUID of the oldest pending questionnaire, or null if no pending questionnaires exist.</returns>
    /// <remarks>
    /// This method supports the workflow management by identifying the next questionnaire
    /// that requires attention from the user. Handles both student and teacher perspectives,
    /// checking appropriate completion timestamps based on user role. Orders by activation
    /// date to ensure FIFO processing of questionnaires.
    /// </remarks>
    public async Task<Guid?> GetIdOfOldestActiveQuestionnaire(Guid id)
    {
        return await _context.ActiveQuestionnaires
            .Where(a => (a.Student != null && a.Student.Guid == id && !a.StudentCompletedAt.HasValue) || 
                       (a.Teacher != null && a.Teacher.Guid == id && !a.TeacherCompletedAt.HasValue))
            .OrderBy(a => a.ActivatedAt)
            .Select(a => (Guid?)a.Id)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Checks whether a user exists in the system using their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the user to check.</param>
    /// <returns>True if the user exists, false otherwise.</returns>
    /// <remarks>
    /// This method provides efficient existence checking using the GUID identifier.
    /// Useful for validation operations before performing user-related actions,
    /// and supports both student and teacher lookups across the inheritance hierarchy.
    /// </remarks>
    public bool UserExists(Guid id)
    {
        return _genericRepository.Count(u => u.Guid == id) != 0;
    }

    /// <summary>
    /// Checks whether a user exists in the system using their primary key identifier.
    /// </summary>
    /// <param name="primaryKey">The primary key (integer) of the user to check.</param>
    /// <returns>True if the user exists, false otherwise.</returns>
    /// <remarks>
    /// This overload provides existence checking using the internal primary key identifier.
    /// Primarily used for internal operations and database relationship validation.
    /// Less commonly used than the GUID-based overload in application logic.
    /// </remarks>
    public bool UserExists(int primaryKey)
    {
        return _genericRepository.Count(u => u.Id == primaryKey) != 0;
    }

    /// <summary>
    /// Adds a new student to the system.
    /// </summary>
    /// <param name="student">The UserAdd DTO containing student information to be added.</param>
    /// <returns>A task representing the asynchronous add operation.</returns>
    /// <remarks>
    /// This method creates a new student record in the database using the StudentModel
    /// derived type. Handles the conversion from DTO to entity model and delegates
    /// to the generic repository for the actual database operation.
    /// </remarks>
    public async Task AddStudentAsync(UserAdd student)
    {
        await _genericRepository.AddAsync(student.ToStudentModel());
    }

    /// <summary>
    /// Adds a new teacher to the system.
    /// </summary>
    /// <param name="teacher">The UserAdd DTO containing teacher information to be added.</param>
    /// <returns>A task representing the asynchronous add operation.</returns>
    /// <remarks>
    /// This method creates a new teacher record in the database using the TeacherModel
    /// derived type. Handles the conversion from DTO to entity model and delegates
    /// to the generic repository for the actual database operation.
    /// </remarks>
    public async Task AddTeacherAsync(UserAdd teacher)
    {
        await _genericRepository.AddAsync(teacher.ToTeacherModel());
    }

    /// <summary>
    /// Searches for students related to a specific teacher by student username.
    /// </summary>
    /// <param name="teacherId">The unique identifier (GUID) of the teacher.</param>
    /// <param name="studentUsernameQuery">The student username or partial username to search for.</param>
    /// <returns>A list of UserBase DTOs representing students related to the teacher that match the username query.</returns>
    /// <exception cref="ArgumentException">Thrown when teacherId is empty or studentUsernameQuery is null/empty.</exception>
    /// <remarks>
    /// This method finds students who have active questionnaires assigned to the specified teacher
    /// and whose username contains the search query. This ensures that teachers can only search
    /// for students they are working with through questionnaires. Returns full user DTOs
    /// for efficient display and selection purposes with complete user information. The search is case-insensitive and uses partial matching.
    /// </remarks>
    public async Task<List<FullUser>> SearchStudentsRelatedToTeacherAsync(Guid teacherId, string studentUsernameQuery)
    {
        if (teacherId == Guid.Empty)
            throw new ArgumentException("Teacher ID cannot be empty", nameof(teacherId));
            
        if (string.IsNullOrWhiteSpace(studentUsernameQuery))
            throw new ArgumentException("Student username query cannot be null or empty", nameof(studentUsernameQuery));

        // Find students that have active questionnaires with this teacher and match the username query
        var students = await _context.ActiveQuestionnaires
            .Where(aq => aq.Teacher != null && aq.Teacher.Guid == teacherId && 
                        aq.Student != null && aq.Student.UserName.Contains(studentUsernameQuery))
            .Select(aq => aq.Student!)
            .Distinct()
            .ToListAsync();

        return students.Select(s => s.ToDto()).ToList();
    
    public Task<int?> GetIdByGuidAsync(Guid guid)
    {
        return _context.Set<TeacherModel>()
            .Where(t => t.Guid == guid)
            .Select(t => (int?)t.Id)
            .FirstOrDefaultAsync();
    }
}
