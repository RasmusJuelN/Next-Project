using Database.Enums;

namespace API.DTO.Requests.ActiveQuestionnaire
{
    /// <summary>
    /// Represents the request payload for fetching questionnaire groups using keyset pagination,
    /// including page size, ordering, filters, and continuation cursor.
    /// </summary>
    /// <remarks>
    /// This request model enables efficient retrieval of paginated questionnaire group data
    /// by allowing clients to specify sorting, filtering, and pagination parameters.
    /// </remarks>
    public record class QuestionnaireGroupKeysetPaginationRequest
    {
        /// <summary>
        /// Gets or sets the maximum number of groups to return per page.
        /// </summary>
        /// <remarks>
        /// Defaults to 5 if not specified.
        /// </remarks>
        public required int PageSize { get; set; } = 5;

        /// <summary>
        /// Gets or sets the ordering option used to sort the questionnaire groups.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="QuestionnaireGroupOrderingOptions.CreatedAtDesc"/> (newest first).
        /// </remarks>
        public QuestionnaireGroupOrderingOptions Order { get; set; } = QuestionnaireGroupOrderingOptions.CreatedAtDesc;

        /// <summary>
        /// Gets or sets an optional title filter applied to group names.
        /// </summary>
        /// <remarks>
        /// Only groups whose names contain the specified title text will be included.
        /// </remarks>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets an optional group ID filter for retrieving a specific questionnaire group.
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// Gets or sets the query cursor used for continuing keyset pagination.
        /// </summary>
        /// <remarks>
        /// The cursor is a composite string containing the creation date and group ID of
        /// the last item from the previous page.
        /// </remarks>
        public string? QueryCursor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include only groups with at least
        /// one questionnaire not yet completed by a student.
        /// </summary>
        public bool? PendingStudent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include only groups with at least
        /// one questionnaire not yet completed by a teacher.
        /// </summary>
        public bool? PendingTeacher { get; set; }
    }
}
