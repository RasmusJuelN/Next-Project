namespace API.DTO.Requests.ActiveQuestionnaire;

public record class AnonymisedResponsesRequest
{
    /// <summary>
    /// The unique identifier of the questionnaire for which anonymised responses are requested.
    /// </summary>
    public required Guid QuestionnaireId { get; init; }

    /// <summary>
    /// A list of unique identifiers for users whose responses should be included in the anonymised dataset.
    /// </summary>
    /// <remarks>
    /// If this list is empty, responses from all users will be included. Should not be mixed with other filters.
    /// </remarks>
    public List<Guid> Users { get; set; } = [];

    /// <summary>
    /// A list of unique identifiers for groups whose responses should be included in the anonymised dataset.
    /// </summary>
    /// <remarks>
    /// If this list is empty, responses from all users will be included. Should not be mixed with other filters.
    /// </remarks>
    public List<Guid> Groups { get; set; } = [];

    /// <summary>
    /// The number of days within which active questionnaires should be grouped together.
    /// Questionnaires activated within this time window will be combined into the same dataset.
    /// </summary>
    /// <remarks>
    /// Default value is 1, meaning questionnaires activated on the same day will be grouped together.
    /// A value of 7 would group questionnaires activated within a week of each other.
    /// </remarks>
    public int GrouppedWithinDays { get; set; } = 1;
}
