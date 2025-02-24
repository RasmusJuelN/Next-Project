namespace API.DTO.Requests.QuestionnaireTemplate;

public record class UpdateTemplate
{
    public required string TemplateTitle { get; set; }
    public List<UpdateQuestion> Questions { get; set; } = [];
}
