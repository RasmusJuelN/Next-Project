namespace API.Models.Requests;

public record class QuestionnaireTemplateQuestionUpdateRequest
{
    public int Id { get; set; }
    public required string Prompt { get; set; }
    public bool AllowCustom { get; set; }
    public List<QuestionnaireTemplateOptionUpdateRequest> Options { get; set; } = [];
}
