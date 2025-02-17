namespace API.Models.Requests;

public record class QuestionnaireTemplateUpdateRequest
{
    public required Guid Id { get; set; }
    public required string TemplateTitle { get; set; }
    public List<QuestionnaireTemplateQuestionUpdateRequest> Questions { get; set; } = [];
}
