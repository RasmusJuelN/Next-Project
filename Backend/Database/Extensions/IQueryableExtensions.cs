using Database.Models;

namespace Database.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<QuestionnaireTemplateModel> OrderByTitleAsc(this IQueryable<QuestionnaireTemplateModel> query)
    {
        return query.OrderBy(q => q.Title).ThenBy(q => q.Id);
    }

    public static IQueryable<QuestionnaireTemplateModel> OrderByTitleDesc(this IQueryable<QuestionnaireTemplateModel> query)
    {
        return query.OrderByDescending(q => q.Title).ThenBy(q => q.Id);
    }

    public static IQueryable<QuestionnaireTemplateModel> OrderByCreatedAtAsc(this IQueryable<QuestionnaireTemplateModel> query)
    {
        return query.OrderBy(q => q.CreatedAt).ThenBy(q => q.Id);
    }

    public static IQueryable<QuestionnaireTemplateModel> OrderByCreatedAtDesc(this IQueryable<QuestionnaireTemplateModel> query)
    {
        return query.OrderByDescending(q => q.CreatedAt).ThenBy(q => q.Id);
    }

    public static IQueryable<ActiveQuestionnaireModel> OrderByTitleAsc(this IQueryable<ActiveQuestionnaireModel> query)
    {
        return query.OrderBy(a => a.Title).ThenBy(a => a.Id);
    }

    public static IQueryable<ActiveQuestionnaireModel> OrderByTitleDesc(this IQueryable<ActiveQuestionnaireModel> query)
    {
        return query.OrderByDescending(q => q.Title).ThenBy(q => q.Id);
    }

    public static IQueryable<ActiveQuestionnaireModel> OrderByActivatedAtAsc(this IQueryable<ActiveQuestionnaireModel> query)
    {
        return query.OrderBy(q => q.ActivatedAt).ThenBy(q => q.Id);
    }

    public static IQueryable<ActiveQuestionnaireModel> OrderByActivatedAtDesc(this IQueryable<ActiveQuestionnaireModel> query)
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
