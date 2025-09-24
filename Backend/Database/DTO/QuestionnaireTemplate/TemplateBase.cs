using System.Text.Json.Serialization;

namespace Database.DTO.QuestionnaireTemplate;

/// <summary>
/// Represents the base properties for a questionnaire template.
/// </summary>
/// <param name="Id">The unique identifier for the questionnaire template.</param>
/// <param name="Title">The title of the questionnaire template.</param>
/// <param name="Description">An optional description providing additional details about the questionnaire template.</param>
/// <param name="CreatedAt">The date and time when the questionnaire template was created.</param>
/// <param name="LastUpdated">The date and time when the questionnaire template was last modified.</param>
/// <param name="IsLocked">Indicates whether the questionnaire template is locked and cannot be modified.</param>
public record class QuestionnaireTemplateBase
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime LastUpdated { get; set; }
    public required bool IsLocked { get; set; }
    public TemplateStatus TemplateStatus { get; init; }
}
