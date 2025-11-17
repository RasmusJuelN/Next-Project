using Database.Enums;

namespace API.DTO.Requests.ActiveQuestionnaire
{
    /// <summary>
    /// Represents the request payload for fetching questionnaire groups using offset pagination,
    /// including page number, page size, ordering, and filters.
    /// </summary>
    /// <remarks>
    /// This request model enables efficient retrieval of paginated questionnaire group data
    /// by allowing clients to specify sorting, filtering, and pagination parameters.
    /// Offset pagination allows direct navigation to any page using page numbers,
    /// making it suitable for user interfaces with traditional pagination controls.
    /// </remarks>
    public record class QuestionnaireGroupOffsetPaginationRequest
    {
        /// <summary>
        /// Gets or sets the page number to retrieve (1-based indexing).
        /// </summary>
        /// <remarks>
        /// Defaults to 1 if not specified. Page numbers start at 1 for the first page.
        /// </remarks>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Gets or sets the maximum number of groups to return per page.
        /// </summary>
        /// <remarks>
        /// Defaults to 5 if not specified. This value determines how many groups
        /// are displayed on each page of results.
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
        /// The search is case-insensitive.
        /// </remarks>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets an optional group ID filter for retrieving a specific questionnaire group.
        /// </summary>
        /// <remarks>
        /// When specified, only the group with the exact matching ID will be returned.
        /// </remarks>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include only groups with at least
        /// one questionnaire not yet completed by a student.
        /// </summary>
        /// <remarks>
        /// When set to true, filters the results to show only groups that have pending student submissions.
        /// </remarks>
        public bool? PendingStudent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include only groups with at least
        /// one questionnaire not yet completed by a teacher.
        /// </summary>
        /// <remarks>
        /// When set to true, filters the results to show only groups that have pending teacher reviews.
        /// </remarks>
        public bool? PendingTeacher { get; set; }
    }
}