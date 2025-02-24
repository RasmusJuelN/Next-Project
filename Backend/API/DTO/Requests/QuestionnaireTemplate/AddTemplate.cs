namespace API.DTO.Requests.QuestionnaireTemplate;

public record class AddTemplate
{
    public required string TemplateTitle { get; set; }
    public required List<AddQuestion> Questions { get; set; }
}