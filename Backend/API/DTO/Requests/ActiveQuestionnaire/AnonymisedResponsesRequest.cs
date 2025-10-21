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
}
