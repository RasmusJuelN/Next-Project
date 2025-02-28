namespace API.DTO.Requests.QuestionnaireTemplate;

public record class AddQuestion
{
    public required string Prompt { get; set; }
    public required bool AllowCustom { get; set; }
    public required List<AddOption> Options { get; set; }
}
