namespace API.DTO.Requests.QuestionnaireTemplate;

public record class AddOption
{
    public int Id { get; set; }
    public int OptionValue { get; set; }
    public required string DisplayText { get; set; }
}
