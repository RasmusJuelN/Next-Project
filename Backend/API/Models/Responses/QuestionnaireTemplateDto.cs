namespace API.Models.Responses;

public record class QuestionnaireTemplateDto : QuestionnaireTemplateBaseDto
{
    public required List<QuestionnaireTemplateQuestionDto> Questions { get; set; }
}
