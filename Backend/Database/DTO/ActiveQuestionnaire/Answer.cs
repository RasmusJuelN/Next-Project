namespace Database.DTO.ActiveQuestionnaire;

public record class Answer
{
    public required int QuestionId { get; set; }
    public required int OptionId { get; set; }
    public string? CustomAnswer { get; set; }
}
