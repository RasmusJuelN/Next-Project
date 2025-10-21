namespace Database.DTO.ActiveQuestionnaire;

public record class AnonymisedResponsesQuestion
{
    public required string Question { get; set; }
    public required List<AnonymisedResponsesAnswer> Answers { get; set; }
}
