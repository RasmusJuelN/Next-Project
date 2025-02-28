namespace API.DTO.Responses.QuestionnaireTemplate;

public record class FetchQuestion
{
    public required int Id { get; set; }
    public required string Prompt { get; set; }
    public required bool AllowCustom { get; set; }
    public required List<FetchOption> Options { get; set; }
}
