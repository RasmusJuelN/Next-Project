namespace Database.DTO.ActiveQuestionnaire;

/// <summary>
/// Represents the data transfer object for adding a new active questionnaire.
/// </summary>
/// <param name="Title">The required title of the questionnaire.</param>
/// <param name="Description">The optional description of the questionnaire.</param>
public record class ActiveQuestionnaireAdd
{
    public required string Title { get; set; }
    public string? Description { get; set; }
}
