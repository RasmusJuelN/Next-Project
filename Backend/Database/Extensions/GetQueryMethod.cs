using System.Reflection;
using Database.Attributes;
using Database.Enums;

namespace Database.Extensions;

public static class QueryMethodExtension
{
    public static IQueryable<T> ApplyQueryOrdering<T>(this TemplateOrderingOptions ordering, IQueryable<T> queryable)
    {
        QueryMethodAttribute queryMethodAttribute = ordering.GetType()
            .GetField(ordering.ToString())?.GetCustomAttribute<QueryMethodAttribute>()
            ?? throw new Exception("QueryMethodAttribute not found");

        string methodName = queryMethodAttribute.QueryMethod;

        MethodInfo methodInfo = typeof(IQueryableExtensions)
            .GetMethod(methodName, BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(IQueryable<T>) }, null)
            ?? throw new Exception("Method not found");

        return methodInfo.Invoke(null, new object[] { queryable }) as IQueryable<T>
               ?? throw new Exception("Query method invocation returned null");
    }

    public static IQueryable<T> ApplyQueryOrdering<T>(this ActiveQuestionnaireOrderingOptions ordering, IQueryable<T> queryable)
    {
        QueryMethodAttribute queryMethodAttribute = ordering.GetType()
            .GetField(ordering.ToString())?.GetCustomAttribute<QueryMethodAttribute>()
            ?? throw new Exception("QueryMethodAttribute not found");

        string methodName = queryMethodAttribute.QueryMethod;

        MethodInfo methodInfo = typeof(IQueryableExtensions)
            .GetMethod(methodName, BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(IQueryable<T>) }, null)
            ?? throw new Exception("Method not found");

        return methodInfo.Invoke(null, new object[] { queryable }) as IQueryable<T>
               ?? throw new Exception("Query method invocation returned null");
    }

    public static IQueryable<T> ApplyQueryOrdering<T>(this QuestionnaireGroupOrderingOptions ordering, IQueryable<T> queryable)
    {
        var queryMethodAttribute = ordering.GetType()
            .GetField(ordering.ToString())?
            .GetCustomAttribute<QueryMethodAttribute>()
            ?? throw new Exception("QueryMethodAttribute not found");

        string methodName = queryMethodAttribute.QueryMethod;

        // Only get method that takes IQueryable<T>
        MethodInfo methodInfo = typeof(IQueryableExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(m => m.Name == methodName)
            .Where(m => m.GetParameters().Length == 1
                        && m.GetParameters()[0].ParameterType.IsAssignableFrom(typeof(IQueryable<T>)))
            .SingleOrDefault() ?? throw new Exception("Method not found");

        return methodInfo.Invoke(null, new object[] { queryable }) as IQueryable<T>
               ?? throw new Exception("Query method invocation returned null");
    }
}
