using System.Reflection;
using Database.Attributes;
using Database.Enums;

namespace Database.Extensions;

public static class QueryMethodExtension
{
    public static IQueryable<T> ApplyQueryOrdering<T>(this TemplateOrderingOptions ordering, IQueryable<T> queryable)
    {
        QueryMethodAttribute queryMethodAttribute = ordering.GetType().GetCustomAttribute<QueryMethodAttribute>() ?? throw new Exception("QueryMethodAttribute not found");
        string methodName = queryMethodAttribute.QueryMethod;
        MethodInfo methodInfo = typeof(IQueryableExtensions).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public) ?? throw new Exception("Method not found");
        return methodInfo.Invoke(null, [queryable]) as IQueryable<T> ?? throw new Exception("Query method invocation returned null");
    }

    public static IQueryable<T> ApplyQueryOrdering<T>(this ActiveQuestionnaireOrderingOptions ordering, IQueryable<T> queryable)
    {
        QueryMethodAttribute queryMethodAttribute = ordering.GetType().GetCustomAttribute<QueryMethodAttribute>() ?? throw new Exception("QueryMethodAttribute not found");
        string methodName = queryMethodAttribute.QueryMethod;
        MethodInfo methodInfo = typeof(IQueryableExtensions).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public) ?? throw new Exception("Method not found");
        return methodInfo.Invoke(null, [queryable]) as IQueryable<T> ?? throw new Exception("Query method invocation returned null");
    }
}
