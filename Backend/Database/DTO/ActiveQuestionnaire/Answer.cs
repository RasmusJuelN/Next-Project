namespace Database.DTO.ActiveQuestionnaire;

/// <summary>
/// Represents an answer to a questionnaire question.
/// </summary>
/// <remarks>
/// This record contains the response data for a single question in an active questionnaire.
/// An answer can be either a selected option (via OptionId) or a custom text response (via CustomAnswer).
/// </remarks>
public record class Answer
{
    public required int QuestionId { get; set; }
    public int? OptionId { get; set; }
    public string? CustomAnswer { get; set; }
}
