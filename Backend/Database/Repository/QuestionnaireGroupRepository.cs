
namespace Database.Repository
{
    /// <summary>
    /// Provides data access operations for managing questionnaire groups, including creation,
    /// retrieval, and paginated queries with optional filtering and ordering.
    /// </summary>
    /// <remarks>
    /// This repository encapsulates all database interactions for <see cref="QuestionnaireGroupModel"/> entities,
    /// supporting keyset pagination for efficient data retrieval in large datasets.
    /// </remarks>
    public class QuestionnaireGroupRepository : IQuestionnaireGroupRepository
    {
        private readonly Context _context;
        private readonly GenericRepository<QuestionnaireGroupModel> _genericRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionnaireGroupRepository"/> class
        /// using the specified database context and logger factory.
        /// </summary>
        /// <param name="context">The EF Core database context used for data access.</param>
        /// <param name="loggerFactory">The logger factory for creating repository-level loggers.</param>
        public QuestionnaireGroupRepository(Context context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _genericRepository = new GenericRepository<QuestionnaireGroupModel>(context, loggerFactory);
        }

        /// <summary>
        /// Adds a new questionnaire group to the database.
        /// </summary>
        /// <param name="group">The <see cref="QuestionnaireGroupModel"/> instance to persist.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method directly adds the entity to the database context and commits the changes.
        /// </remarks>
        public async Task AddAsync(QuestionnaireGroupModel group)
        {
            _context.Set<QuestionnaireGroupModel>().Add(group);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a questionnaire group by its unique identifier.
        /// </summary>
        /// <param name="groupId">The GUID of the questionnaire group to retrieve.</param>
        /// <returns>
        /// The <see cref="QuestionnaireGroupModel"/> instance if found; otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>
        /// Uses <see cref="DbSet.FindAsync(object[])"/> for efficient primary key lookups.
        /// </remarks>
        public async Task<QuestionnaireGroupModel> GetByIdAsync(Guid groupId)
        {
            return await _context.Set<QuestionnaireGroupModel>().FindAsync(groupId);
        }
        public async Task<List<QuestionnaireGroupModel>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.Set<QuestionnaireGroupModel>()
                .Where(g => ids.Contains(g.GroupId))
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all questionnaire groups with their related questionnaires,
        /// students, teachers, and templates included.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="QuestionnaireGroupModel"/> instances with all related data eagerly loaded.
        /// </returns>
        /// <remarks>
        /// This method performs multiple <c>Include</c> and <c>ThenInclude</c> operations
        /// to fully hydrate navigation properties for immediate use.
        /// </remarks>
        public async Task<IEnumerable<QuestionnaireGroupModel>> GetAllAsync()
        {
            return await _context.QuestionnaireGroups
                .Include(g => g.Questionnaires)
                    .ThenInclude(q => q.Student)
                .Include(g => g.Questionnaires)
                    .ThenInclude(q => q.Teacher)
                .Include(g => g.Questionnaires)
                    .ThenInclude(q => q.QuestionnaireTemplate)
                .ToListAsync();
        }

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
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description>A list of <see cref="QuestionnaireGroupModel"/> entities that match the query.</description></item>
        /// <item><description>The total count of all entities matching the filter (before pagination).</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// Keyset pagination ensures stable and efficient page navigation by using creation date
        /// and group ID as continuation markers, avoiding performance issues with large offsets.
        /// Related templates, students, and teachers are eagerly loaded for immediate use.
        /// </remarks>
        public async Task<(List<QuestionnaireGroupModel>, int)> PaginationQueryWithKeyset(
            int amount,
                    QuestionnaireGroupOrderingOptions sortOrder,
                    string? titleQuery = null,
                    Guid? groupId = null,
                    bool? pendingStudent = false,
                    bool? pendingTeacher = false,
                    int? teacherFK = null,
                    int? pageNumber = 1)
        {
            IQueryable<QuestionnaireGroupModel> query = _genericRepository.GetAsQueryable();

            query = sortOrder.ApplyQueryOrdering(query);

            if (!string.IsNullOrEmpty(titleQuery))
            {
                query = query.Where(g => g.Name.Contains(titleQuery));
            }

            if (groupId is not null)
            {
                query = query.Where(g => g.GroupId == groupId);
            }

            if (pendingStudent == true)
                query = query.Where(g => g.Questionnaires.Any(q => !q.StudentCompletedAt.HasValue));

            if (pendingTeacher == true)
                query = query.Where(g => g.Questionnaires.Any(q => !q.TeacherCompletedAt.HasValue));

            if (teacherFK.HasValue)
            {
                query = query.Where(g => g.Questionnaires.Any(q => q.TeacherFK == teacherFK.Value));
            }

            int totalCount = await query.CountAsync();


            // Offset-based pagination
            int skip = (pageNumber.Value - 1) * amount;
            query = query.Skip(skip).Take(amount);

            List<QuestionnaireGroupModel> groupEntities = await query
        .Include(g => g.Template)
        .Include(g => g.Questionnaires)
            .ThenInclude(q => q.Student)
        .Include(g => g.Questionnaires)
            .ThenInclude(q => q.Teacher)
        .ToListAsync();

            if (teacherFK.HasValue)
            {
                foreach (var grp in groupEntities)
                {
                    grp.Questionnaires = grp.Questionnaires
                        .Where(q => q.TeacherFK == teacherFK.Value)
                        .ToList();
                }
            }

            return (groupEntities, totalCount);
        }



    }
}