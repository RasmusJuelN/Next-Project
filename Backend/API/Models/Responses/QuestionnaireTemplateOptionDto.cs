namespace API.Models.Responses;

public record class QuestionnaireTemplateOptionDto
{
    public required int OptionValue { get; set; }
    public required string DisplayText { get; set; }
}
