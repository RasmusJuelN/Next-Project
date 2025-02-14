using System.Reflection;
using API.Attributes;
using API.Enums;
using Microsoft.OpenApi.Extensions;

namespace API.Extensions;

public static class QueryMethodExtension
{
    public static IQueryable<T> ApplyQueryOrdering<T>(this QuestionnaireBaseTemplateOrdering ordering, IQueryable<T> queryable)
    {
        QueryMethodAttribute queryMethodAttribute = ordering.GetAttributeOfType<QueryMethodAttribute>();
        string methodName = queryMethodAttribute.QueryMethod;
        MethodInfo methodInfo = typeof(IQueryableExtensions).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public) ?? throw new Exception("Method not found");
        return methodInfo.Invoke(null, [queryable]) as IQueryable<T> ?? throw new Exception("Query method invocation returned null");
    }
}
