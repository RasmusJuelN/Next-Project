namespace Database.DTO.ActiveQuestionnaire;

public record class AnswerSubmission
{
    public required List<Answer> Answers { get; set; }
}
