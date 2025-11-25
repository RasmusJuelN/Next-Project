
namespace API.DTO.Responses.ActiveQuestionnaire
{
    /// <summary>
    /// Represents the result of a keyset-paginated query for questionnaire groups,
    /// including the retrieved groups, a continuation cursor, and the total entity count.
    /// </summary>
    /// <remarks>
    /// This DTO is used to efficiently transfer paginated questionnaire group data to clients.
    /// The <see cref="QueryCursor"/> provides the necessary information to fetch the next page of results.
    /// </remarks>
    public record class QuestionnaireGroupKeysetPaginationResult
    {
        public required List<QuestionnaireGroupResult> Groups { get; set; }
        public string? QueryCursor { get; set; }
        public int TotalCount { get; set; }
    }
}
