
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

    /// <summary>
    /// Asynchronously retrieves a list of all log event IDs from the application log repository.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of integers
    /// representing the unique identifiers of log events.
    /// </returns>
    Task<List<EventId>> GetLogEventsAsync();

    /// <summary>
    /// Retrieves a list of application logs based on the specified query criteria.
    /// </summary>
    /// <param name="logQuery">The query parameters used to filter and retrieve application logs.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of application logs that match the query criteria.</returns>
    Task<List<ApplicationLog>> GetApplicationLogsAsync(ApplicationLogQuery logQuery);
}
