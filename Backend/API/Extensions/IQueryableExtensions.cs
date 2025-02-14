using Database.Models;

namespace API.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<QuestionnaireTemplateModel> OrderByTitleAsc(this IQueryable<QuestionnaireTemplateModel> query)
    {
        return query.OrderBy(q => q.TemplateTitle).ThenBy(q => q.Id);
    }

    public static IQueryable<QuestionnaireTemplateModel> OrderByTitleDesc(this IQueryable<QuestionnaireTemplateModel> query)
    {
        return query.OrderByDescending(q => q.TemplateTitle).ThenBy(q => q.Id);
    }

    public static IQueryable<QuestionnaireTemplateModel> OrderByCreatedAtAsc(this IQueryable<QuestionnaireTemplateModel> query)
    {
        return query.OrderBy(q => q.CreatedAt).ThenBy(q => q.Id);
    }

    public static IQueryable<QuestionnaireTemplateModel> OrderByCreatedAtDesc(this IQueryable<QuestionnaireTemplateModel> query)
    {
        return query.OrderByDescending(q => q.CreatedAt).ThenBy(q => q.Id);
    }
}
