namespace API.DTO.Responses.QuestionnaireTemplate;

public record class FetchOption
{
    public required int Id { get; set; }
    public required int OptionValue { get; set; }
    public required string DisplayText { get; set; }
}
