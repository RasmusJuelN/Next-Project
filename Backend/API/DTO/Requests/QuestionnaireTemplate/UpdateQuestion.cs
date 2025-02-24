namespace API.DTO.Requests.QuestionnaireTemplate;

public record class UpdateQuestion
{
    public int Id { get; set; }
    public required string Prompt { get; set; }
    public bool AllowCustom { get; set; }
    public List<UpdateOption> Options { get; set; } = [];
}
