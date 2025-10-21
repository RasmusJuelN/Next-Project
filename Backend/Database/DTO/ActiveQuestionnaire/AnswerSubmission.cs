namespace Database.DTO.ActiveQuestionnaire;

/// <summary>
/// Represents a submission containing a collection of answers for a questionnaire.
/// </summary>
/// <remarks>
/// This record is used as a data transfer object (DTO) to encapsulate multiple answers
/// that are submitted together as part of completing an active questionnaire.
/// </remarks>
public record class AnswerSubmission
{
    public required List<Answer> Answers { get; set; }
}
