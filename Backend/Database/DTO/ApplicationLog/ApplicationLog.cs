
namespace Database.DTO.ApplicationLog;

/// <summary>
/// Represents an application log entry that captures logging information including messages, severity levels, and metadata.
/// </summary>
/// <remarks>
/// This record is used to store structured log data with automatic timestamp tracking and categorization.
/// It supports different log levels and can optionally include exception details for error scenarios.
/// </remarks>
public record class ApplicationLog
{
    /// <summary>
    /// Gets or sets the unique identifier for the log entry.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the log message content. This property is required.
    /// </summary>
    public required string Message { get; set; }
    
    /// <summary>
    /// Gets or sets the severity level of the log entry. This property is required.
    /// </summary>
    public required LogLevel LogLevel { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when the log entry was created. Defaults to the current UTC time.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets the event identifier associated with the log entry. This property is required.
    /// </summary>
    public required int EventId { get; set; }
    
    /// <summary>
    /// Gets or sets the category or source of the log entry. This property is required.
    /// </summary>
    public required string Category { get; set; }
    
    /// <summary>
    /// Gets or sets the exception details if the log entry is related to an error condition.
    /// This property is optional and can be null.
    /// </summary>
    public string? Exception { get; set; }
}
