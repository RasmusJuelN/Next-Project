namespace Database.DTO.ActiveQuestionnaire;

public record class AnonymisedSurveyResults
{
    /// <summary>
    /// The title of the dataset containing the anonymised responses. This can for example be the date the dataset was
    /// generated or a custom title provided by the requester.
    /// </summary>
    public required string DatasetTitle { get; set; }


    /// <summary>
    /// The number of participants who contributed to this dataset.
    /// </summary>
    public required int ParticipantCount { get; set; }

    /// <summary>
    /// A dictionary where each key is a question, and the value is another dictionary mapping each question
    /// answer to the number of times it was given.
    /// </summary>
    /// <remarks>
    /// For example, for a question "What is your favorite color?" the inner dictionary might look like:
    /// <code>
    /// {
    ///   "Red": 10,
    ///   "Blue": 15,
    ///   "Green": 5
    /// }
    /// </code>
    /// This indicates that "Red" was chosen 10 times, "Blue" 15
    /// times, and "Green" 5 times.
    /// </remarks>
    public required List<AnonymisedResponsesQuestion> AnonymisedResponses { get; set; }
}
