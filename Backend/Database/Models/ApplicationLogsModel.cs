using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Logging;

namespace Database.Models;

/// <summary>
/// Represents an application log entry stored in the database for monitoring and debugging purposes.
/// Captures structured logging information including level, category, and exception details.
/// </summary>
/// <remarks>
/// This model stores log entries from the application's logging infrastructure, providing persistent
/// storage for monitoring, debugging, and audit trail purposes. Log entries are categorized and
/// timestamped for efficient querying and analysis.
/// </remarks>
[Table("ApplicationLogs")]
public class ApplicationLogsModel
{
    /// <summary>
    /// Gets or sets the unique identifier for this log entry.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the primary log message content.
    /// </summary>
    /// <remarks>
    /// Contains the main log message.
    /// Should provide clear, concise information about the logged event or condition.
    /// </remarks>
    [Required]
    public required string Message { get; set; }

    /// <summary>
    /// Gets or sets the severity level of this log entry using Microsoft.Extensions.Logging.LogLevel.
    /// </summary>
    /// <remarks>
    /// Determines the importance and severity of the log entry (Trace, Debug, Information, Warning, Error, Critical).
    /// Used for filtering and alerting based on log severity.
    /// </remarks>
    [Required]
    public required LogLevel LogLevel { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this log entry was created.
    /// </summary>
    /// <remarks>
    /// Default value is configured in Fluent API to use the current timestamp at creation.
    /// Provides chronological ordering and enables time-based log analysis and retention policies.
    /// </remarks>
    [Required]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the event identifier associated with this log entry.
    /// </summary>
    /// <remarks>
    /// Provides a way to categorize and group related log entries by event type.
    /// Useful for tracking specific application events and creating structured monitoring.
    /// </remarks>
    [Required]
    public required int EventId { get; set; }

    /// <summary>
    /// Gets or sets the description of the event associated with this log entry.
    /// </summary>
    /// <remarks>
    /// Provides additional context about the event being logged, and which type it is.
    /// </remarks>
    [Required]
    public required string EventDescription { get; set; }

    /// <summary>
    /// Gets or sets the category or source component that generated this log entry.
    /// </summary>
    /// <remarks>
    /// Identifies the application component, class, or subsystem that generated the log entry.
    /// Enables filtering and analysis by functional area or component type.
    /// </remarks>
    [Required]
    public required string Category { get; set; }

    /// <summary>
    /// Gets or sets the optional exception details if this log entry is related to an error.
    /// </summary>
    /// <remarks>
    /// Contains serialized exception information including stack traces and inner exceptions.
    /// Null for non-error log entries.
    /// </remarks>
    public string? Exception { get; set; }
}
