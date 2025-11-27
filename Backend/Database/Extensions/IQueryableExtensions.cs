
namespace Database.Extensions;

/// <summary>
/// Provides extension methods for applying consistent ordering to IQueryable collections.
/// These methods define standard sorting patterns used throughout the application for pagination and data retrieval.
/// </summary>
public static class IQueryableExtensions
{
    /// <summary>
    /// Orders QuestionnaireTemplateModel entities by title in ascending order with ID as secondary sort.
    /// </summary>
    /// <param name="query">The IQueryable of QuestionnaireTemplateModel to order.</param>
    /// <returns>
    /// An IQueryable ordered by title ascending, then by ID ascending for consistent pagination results.
    /// </returns>
    /// <remarks>
    /// This method provides a stable sort order for questionnaire templates, ensuring consistent results
    /// across paginated queries. The secondary sort by ID ensures deterministic ordering when titles are identical.
    /// </remarks>
    public static IQueryable<QuestionnaireTemplateModel> OrderByTitleAsc(this IQueryable<QuestionnaireTemplateModel> query)
    {
        return query.OrderBy(q => q.Title).ThenBy(q => q.Id);
    }

    /// <summary>
    /// Orders QuestionnaireTemplateModel entities by title in descending order with ID as secondary sort.
    /// </summary>
    /// <param name="query">The IQueryable of QuestionnaireTemplateModel to order.</param>
    /// <returns>
    /// An IQueryable ordered by title descending, then by ID descending for consistent pagination results.
    /// </returns>
    /// <remarks>
    /// This method provides a stable sort order for questionnaire templates in reverse alphabetical order,
    /// ensuring consistent results across paginated queries. The secondary sort by ID maintains deterministic ordering.
    /// </remarks>
    public static IQueryable<QuestionnaireTemplateModel> OrderByTitleDesc(this IQueryable<QuestionnaireTemplateModel> query)
    {
        return query.OrderByDescending(q => q.Title).ThenBy(q => q.Id);
    }

    /// <summary>
    /// Orders QuestionnaireTemplateModel entities by creation date in ascending order with ID as secondary sort.
    /// </summary>
    /// <param name="query">The IQueryable of QuestionnaireTemplateModel to order.</param>
    /// <returns>
    /// An IQueryable ordered by CreatedAt ascending, then by ID ascending for consistent pagination results.
    /// </returns>
    /// <remarks>
    /// This method provides chronological ordering of questionnaire templates from oldest to newest.
    /// The secondary sort by ID ensures deterministic ordering when creation timestamps are identical.
    /// </remarks>
    public static IQueryable<QuestionnaireTemplateModel> OrderByCreatedAtAsc(this IQueryable<QuestionnaireTemplateModel> query)
    {
        return query.OrderBy(q => q.CreatedAt).ThenBy(q => q.Id);
    }

    /// <summary>
    /// Orders QuestionnaireTemplateModel entities by creation date in descending order with ID as secondary sort.
    /// </summary>
    /// <param name="query">The IQueryable of QuestionnaireTemplateModel to order.</param>
    /// <returns>
    /// An IQueryable ordered by CreatedAt descending, then by ID descending for consistent pagination results.
    /// </returns>
    /// <remarks>
    /// This method provides reverse chronological ordering of questionnaire templates from newest to oldest.
    /// The secondary sort by ID ensures deterministic ordering when creation timestamps are identical.
    /// </remarks>
    public static IQueryable<QuestionnaireTemplateModel> OrderByCreatedAtDesc(this IQueryable<QuestionnaireTemplateModel> query)
    {
        return query.OrderByDescending(q => q.CreatedAt).ThenBy(q => q.Id);
    }

    /// <summary>
    /// Orders the collection of active questionnaire models by title in ascending order, 
    /// with ID as a secondary sort criteria for consistent ordering.
    /// </summary>
    /// <param name="query">The queryable collection of active questionnaire models to order.</param>
    /// <returns>An ordered queryable collection sorted by title ascending, then by ID ascending.</returns>
    public static IQueryable<StandardActiveQuestionnaireModel> OrderByTitleAsc(this IQueryable<StandardActiveQuestionnaireModel> query)
    {
        return query.OrderBy(a => a.Title).ThenBy(a => a.Id);
    }

    /// <summary>
    /// Orders the queryable collection of active questionnaires by title in descending order, 
    /// then by ID in ascending order as a secondary sort criteria.
    /// </summary>
    /// <param name="query">The queryable collection of ActiveQuestionnaireModel to order.</param>
    /// <returns>An ordered queryable collection with questionnaires sorted by title (Z-A), then by ID (ascending).</returns>
    public static IQueryable<StandardActiveQuestionnaireModel> OrderByTitleDesc(this IQueryable<StandardActiveQuestionnaireModel> query)
    {
        return query.OrderByDescending(q => q.Title).ThenBy(q => q.Id);
    }

    /// <summary>
    /// Orders ActiveQuestionnaireModel entities by activation date in ascending order with ID as secondary sort.
    /// </summary>
    /// <param name="query">The IQueryable of ActiveQuestionnaireModel to order.</param>
    /// <returns>
    /// An IQueryable ordered by ActivatedAt ascending, then by ID ascending for consistent pagination results.
    /// </returns>
    /// <remarks>
    /// This method provides chronological ordering of active questionnaires from earliest to latest activation.
    /// The secondary sort by ID ensures deterministic ordering when activation timestamps are identical.
    /// </remarks>
    public static IQueryable<StandardActiveQuestionnaireModel> OrderByActivatedAtAsc(this IQueryable<StandardActiveQuestionnaireModel> query)
    {
        return query.OrderBy(q => q.ActivatedAt).ThenBy(q => q.Id);
    }

    /// <summary>
    /// Orders ActiveQuestionnaireModel entities by activation date in descending order with ID as secondary sort.
    /// </summary>
    /// <param name="query">The IQueryable of ActiveQuestionnaireModel to order.</param>
    /// <returns>
    /// An IQueryable ordered by ActivatedAt descending, then by ID descending for consistent pagination results.
    /// </returns>
    /// <remarks>
    /// This method provides reverse chronological ordering of active questionnaires from latest to earliest activation.
    /// The secondary sort by ID ensures deterministic ordering when activation timestamps are identical.
    /// </remarks>
    public static IQueryable<StandardActiveQuestionnaireModel> OrderByActivatedAtDesc(this IQueryable<StandardActiveQuestionnaireModel> query)
    {
        return query.OrderByDescending(q => q.ActivatedAt).ThenBy(q => q.Id);
    }

    // Groups
    public static IQueryable<QuestionnaireGroupModel> OrderByNameAsc(this IQueryable<QuestionnaireGroupModel> query)
    {
        return query.OrderBy(g => g.Name).ThenBy(g => g.GroupId);
    }

    public static IQueryable<QuestionnaireGroupModel> OrderByNameDesc(this IQueryable<QuestionnaireGroupModel> query)
    {
        return query.OrderByDescending(g => g.Name).ThenBy(g => g.GroupId);
    }

    public static IQueryable<QuestionnaireGroupModel> OrderByCreatedAtAsc(this IQueryable<QuestionnaireGroupModel> query)
    {
        return query.OrderBy(g => g.CreatedAt).ThenBy(g => g.GroupId);
    }

    public static IQueryable<QuestionnaireGroupModel> OrderByCreatedAtDesc(this IQueryable<QuestionnaireGroupModel> query)
    {
        return query.OrderByDescending(g => g.CreatedAt).ThenBy(g => g.GroupId);
    }
}
