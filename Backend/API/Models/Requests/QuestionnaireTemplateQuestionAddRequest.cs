namespace API.Models.Requests;

public class QuestionnaireTemplateQuestionAddRequest
{
    public required string Prompt { get; set; }
    public required bool AllowCustom { get; set; }
    public required List<QuestionnaireTemplateOptionAddRequest> QuestionnaireTemplateOptions { get; set; }
}
