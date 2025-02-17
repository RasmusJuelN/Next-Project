namespace API.Models.Responses;

public record class QuestionnaireTemplateDto
{
    public required Guid Id { get; set; }
    public required string TemplateTitle { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsLocked { get; set; }
    public required List<QuestionnaireTemplateQuestionDto> Questions { get; set; }
}
