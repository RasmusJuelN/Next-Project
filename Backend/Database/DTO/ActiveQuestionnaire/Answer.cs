namespace Database.DTO.ActiveQuestionnaire;

public record class Answer
{
    public required int QuestionId { get; set; }
    public int? OptionId { get; set; }
    public string? CustomAnswer { get; set; }
}
