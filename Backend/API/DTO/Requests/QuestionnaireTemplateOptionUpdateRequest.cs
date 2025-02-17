namespace API.Models.Requests;

public record class QuestionnaireTemplateOptionUpdateRequest
{
    public int Id { get; set; }
    public int OptionValue { get; set; }
    public required string DisplayText { get; set; }
}
