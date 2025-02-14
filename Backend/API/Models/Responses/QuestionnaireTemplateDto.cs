namespace API.Models.Responses;

public record class QuestionnaireTemplateDto : QuestionnaireTemplateBaseDto.TemplateBase
{
    public required List<QuestionnaireTemplateQuestionDto> Questions { get; set; }
}
