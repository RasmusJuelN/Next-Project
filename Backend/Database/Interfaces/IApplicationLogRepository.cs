using Database.DTO.ApplicationLog;

namespace Database.Interfaces;

/// <summary>
/// Defines the contract for application log repository operations.
/// Manages the persistence and retrieval of application logging data for monitoring and debugging purposes.
/// </summary>
public interface IApplicationLogRepository
{
    /// <summary>
    /// Adds a single application log entry to the database.
    /// </summary>
    /// <param name="applicationLog">The ApplicationLog DTO containing log information to persist.</param>
    /// <remarks>
    /// Use this method for individual log entries. For bulk operations, prefer AddRangeAsync for better performance.
    /// </remarks>
    Task AddAsync(ApplicationLog applicationLog);

    /// <summary>
    /// Adds multiple application log entries to the database in a batch operation.
    /// </summary>
    /// <param name="applicationLogs">A list of ApplicationLog DTOs to persist.</param>
    /// <remarks>
    /// This method provides better performance for bulk log operations by reducing database round trips.
    /// Recommended for scenarios where multiple log entries need to be persisted simultaneously.
    /// </remarks>
    Task AddRangeAsync(List<ApplicationLog> applicationLogs);

    /// <summary>
    /// Retrieves all distinct log categories currently stored in the database.
    /// </summary>
    /// <returns>A list of strings representing unique log categories.</returns>
    /// <remarks>
    /// Used for filtering and categorization in log viewing interfaces.
    /// Categories help organize logs by functional area or component type.
    /// </remarks>
    Task<List<string>> GetLogCategoriesAsync();
}
