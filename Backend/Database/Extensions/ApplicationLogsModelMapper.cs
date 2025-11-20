using Database.DTO.ApplicationLog;
using Database.Models;

namespace Database.Extensions;

/// <summary>
/// Provides extension methods for mapping ApplicationLog DTOs to ApplicationLogsModel entities.
/// This class handles the conversion from data transfer objects to database models for application logging.
/// </summary>
public static class ApplicationLogsModelMapper
{
    /// <summary>
    /// Converts an ApplicationLog DTO to an ApplicationLogsModel entity for database storage.
    /// </summary>
    /// <param name="applicationLog">The ApplicationLog DTO to convert.</param>
    /// <returns>
    /// An ApplicationLogsModel entity containing all log information including message, log level,
    /// timestamp, event ID, category, and exception details ready for database persistence.
    /// </returns>
    /// <remarks>
    /// This method performs a direct mapping between the DTO and entity, preserving all log data
    /// for storage in the application logs table. The mapped entity can be directly saved to the
    /// database context without additional processing.
    /// </remarks>
    public static ApplicationLogsModel ToModel(this ApplicationLog applicationLog)
    {
        return new()
        {
            Message = applicationLog.Message,
            LogLevel = applicationLog.LogLevel,
            Timestamp = applicationLog.Timestamp,
            EventId = applicationLog.EventId,
            EventDescription = applicationLog.EventDescription,
            Category = applicationLog.Category,
            Exception = applicationLog.Exception
        };
    }
}
