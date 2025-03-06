namespace API.DTO.Responses.QuestionnaireTemplate;

public record class FetchTemplate : FetchTemplateBase
{
    public required List<FetchQuestion> Questions { get; set; }
}
