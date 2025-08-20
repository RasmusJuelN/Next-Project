using System.Text.Json.Serialization;

namespace Database.DTO.QuestionnaireTemplate;

public record class QuestionnaireTemplateBase
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime LastUpdated { get; set; }
    public required bool IsLocked { get; set; }
    
    [JsonPropertyName("draftStatus")]
    public TemplateStatus DraftStatus { get; init; }
}
