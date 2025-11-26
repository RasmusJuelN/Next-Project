namespace API.DTO.Responses.ActiveQuestionnaire
{
    /// <summary>
    /// Represents the result of an offset-paginated query for questionnaire groups,
    /// including the retrieved groups, current page number, total pages, and total entity count.
    /// </summary>
    /// <remarks>
    /// This DTO is used to efficiently transfer paginated questionnaire group data to clients.
    /// The <see cref="CurrentPage"/> and <see cref="TotalPages"/> provide information for
    /// direct page navigation, allowing users to jump to any page within the result set.
    /// Offset pagination is suitable for use cases requiring random page access and
    /// consistent total count display.
    /// </remarks>
    public record class QuestionnaireGroupOffsetPaginationResult
    {
        public required List<QuestionnaireGroupResult> Groups { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
}
