
namespace Database.Interfaces
{
    public interface IQuestionnaireGroupRepository
    {
        /// <summary>
        /// Adds a new questionnaire group to the database.
        /// </summary>
        /// <param name="group">The <see cref="QuestionnaireGroupModel"/> instance to persist.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method persists the specified questionnaire group entity to the underlying data store.
        /// </remarks>
        Task AddAsync(QuestionnaireGroupModel group);

        /// <summary>
        /// Retrieves all questionnaire groups along with their related questionnaires,
        /// students, teachers, and templates.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation, containing a collection of
        /// <see cref="QuestionnaireGroupModel"/> entities with their related data eagerly loaded.
        /// </returns>
        /// <remarks>
        /// This operation loads all navigation properties for immediate consumption.
        /// </remarks>
        Task<IEnumerable<QuestionnaireGroupModel>> GetAllAsync();

        /// <summary>
        /// Retrieves a questionnaire group by its unique identifier.
        /// </summary>
        /// <param name="groupId">The GUID of the questionnaire group to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation, containing the
        /// <see cref="QuestionnaireGroupModel"/> if found; otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>
        /// Uses a primary key lookup for efficient retrieval.
        /// </remarks>
        Task<QuestionnaireGroupModel> GetByIdAsync(Guid groupId);
        Task<List<QuestionnaireGroupModel>> GetByIdsAsync(IEnumerable<Guid> ids);
        /// <summary>
        /// Retrieves a paginated list of questionnaire groups using keyset pagination with
        /// optional filtering, ordering, and cursor-based continuation.
        /// </summary>
        /// <param name="amount">The maximum number of results to return.</param>
        /// <param name="sortOrder">The ordering criteria for the results.</param>
        /// <param name="cursorIdPosition">An optional cursor group ID to continue pagination.</param>
        /// <param name="cursorCreatedAtPosition">An optional cursor creation date to continue pagination.</param>
        /// <param name="titleQuery">An optional text filter applied to group names.</param>
        /// <param name="groupId">An optional group ID filter for exact matches.</param>
        /// <param name="pendingStudent">If true, only groups with at least one incomplete student questionnaire are returned.</param>
        /// <param name="pendingTeacher">If true, only groups with at least one incomplete teacher questionnaire are returned.</param>
        /// <returns>
        /// A task that represents the asynchronous operation, containing a tuple with:
        /// <list type="bullet">
        /// <item><description>A list of <see cref="QuestionnaireGroupModel"/> entities that match the query.</description></item>
        /// <item><description>The total count of all entities matching the filter (before pagination).</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// Keyset pagination ensures stable and efficient page navigation by using creation date
        /// and group ID as continuation markers, avoiding performance issues with large offsets.
        /// </remarks>
        Task<(List<QuestionnaireGroupModel>, int)> PaginationQueryWithKeyset(
            int amount,
            QuestionnaireGroupOrderingOptions sortOrder,
            string? titleQuery = null,
            Guid? groupId = null,
            bool? pendingStudent = false,
            bool? pendingTeacher = false,
            int? teacherFK = null,
            int? pageNumber = null,
            ActiveQuestionnaireType? activeQuestionnaireType = null);


    }
}
