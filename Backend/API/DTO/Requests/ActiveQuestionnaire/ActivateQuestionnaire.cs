namespace API.DTO.Requests.ActiveQuestionnaire;

/// <summary>
/// Represents a request to activate a questionnaire for a specific student and teacher using a given template.
/// </summary>
/// <param name="StudentId">The unique identifier of the student for whom the questionnaire is being activated.</param>
/// <param name="TeacherId">The unique identifier of the teacher for whom the questionnaire is being activated.</param>
/// <param name="TemplateId">The unique identifier of the questionnaire template to be used.</param>
public record class ActivateQuestionnaire
{
    public required Guid StudentId { get; set; }
    public required Guid TeacherId { get; set; }
    public required Guid TemplateId { get; set; }
}
