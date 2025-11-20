namespace Database.DTO.ActiveQuestionnaire;

public record class SurveyResponseSummary
{
    /// <summary>
    /// The title of the questionnaire for which the anonymised responses are provided.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// The description of the questionnaire for which the anonymised responses are provided.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// A list of datasets containing anonymised responses.
    /// </summary>
    public required List<AnonymisedSurveyResults> AnonymisedResponseDataSet { get; set; }
}
