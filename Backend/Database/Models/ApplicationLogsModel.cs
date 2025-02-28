using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Logging;

namespace Database.Models;

[Table("ApplicationLogs")]
public class ApplicationLogsModel
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public required string Message { get; set; }
    
    [Required]
    public required LogLevel LogLevel { get; set; }
    
    // Default value configured in Fluent API
    [Required]
    public DateTime Timestamp { get; set; }
    
    [Required]
    public required int EventId { get; set; }
    
    [Required]
    public required string Category { get; set; }
    
    [MaxLength(5000)]
    public string? Exception { get; set; }
}
