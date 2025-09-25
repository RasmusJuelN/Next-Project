namespace Database.DTO.ActiveQuestionnaire;

public record class AnonymisedResponsesAnswer
{
    public required string Answer { get; set; }
    public required int Count { get; set; }
}
