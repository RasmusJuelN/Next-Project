namespace Database.DTO.ApplicationLog;

public record class ApplicationLogQuery
{
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
    public List<string> Categories { get; set; } = [];
    public List<int> Events { get; set; } = [];
}