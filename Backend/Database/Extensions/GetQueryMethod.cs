using System.Reflection;
using Database.Attributes;
using Database.Enums;

namespace Database.Extensions;

/// <summary>
/// Provides extension methods for applying dynamic query ordering based on enum values with QueryMethod attributes.
/// This class uses reflection to dynamically invoke query methods based on metadata attributes.
/// </summary>
public static class QueryMethodExtension
{
    /// <summary>
    /// Applies ordering to an IQueryable based on the specified TemplateOrderingOptions enum value.
    /// </summary>
    /// <typeparam name="T">The entity type being queried.</typeparam>
    /// <param name="ordering">The TemplateOrderingOptions enum value that specifies the ordering method.</param>
    /// <param name="queryable">The IQueryable to apply ordering to.</param>
    /// <returns>
    /// An IQueryable with the specified ordering applied, based on the QueryMethod attribute
    /// associated with the enum value.
    /// </returns>
    /// <remarks>
    /// This method uses reflection to discover the QueryMethodAttribute on the enum value and
    /// dynamically invokes the corresponding static method from IQueryableExtensions. The enum
    /// value must have a QueryMethodAttribute that specifies a valid method name.
    /// </remarks>
    /// <exception cref="Exception">
    /// Thrown when the QueryMethodAttribute is not found on the enum value, when the specified
    /// method is not found in IQueryableExtensions, or when the method invocation returns null.
    /// </exception>
    public static IQueryable<T> ApplyQueryOrdering<T>(this TemplateOrderingOptions ordering, IQueryable<T> queryable)
    {
        QueryMethodAttribute queryMethodAttribute = ordering.GetType().GetField(ordering.ToString())?.GetCustomAttribute<QueryMethodAttribute>() ?? throw new Exception("QueryMethodAttribute not found");
        string methodName = queryMethodAttribute.QueryMethod;
        MethodInfo methodInfo = typeof(IQueryableExtensions).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public) ?? throw new Exception("Method not found");
        return methodInfo.Invoke(null, [queryable]) as IQueryable<T> ?? throw new Exception("Query method invocation returned null");
    }

    /// <summary>
    /// Applies ordering to an IQueryable based on the specified ActiveQuestionnaireOrderingOptions enum value.
    /// </summary>
    /// <typeparam name="T">The entity type being queried.</typeparam>
    /// <param name="ordering">The ActiveQuestionnaireOrderingOptions enum value that specifies the ordering method.</param>
    /// <param name="queryable">The IQueryable to apply ordering to.</param>
    /// <returns>
    /// An IQueryable with the specified ordering applied, based on the QueryMethod attribute
    /// associated with the enum value.
    /// </returns>
    /// <remarks>
    /// This method uses reflection to discover the QueryMethodAttribute on the enum value and
    /// dynamically invokes the corresponding static method from IQueryableExtensions. The enum
    /// value must have a QueryMethodAttribute that specifies a valid method name for active
    /// questionnaire ordering operations.
    /// </remarks>
    /// <exception cref="Exception">
    /// Thrown when the QueryMethodAttribute is not found on the enum value, when the specified
    /// method is not found in IQueryableExtensions, or when the method invocation returns null.
    /// </exception>
    public static IQueryable<T> ApplyQueryOrdering<T>(this ActiveQuestionnaireOrderingOptions ordering, IQueryable<T> queryable)
    {
        QueryMethodAttribute queryMethodAttribute = ordering.GetType().GetField(ordering.ToString())?.GetCustomAttribute<QueryMethodAttribute>() ?? throw new Exception("QueryMethodAttribute not found");
        string methodName = queryMethodAttribute.QueryMethod;
        MethodInfo methodInfo = typeof(IQueryableExtensions).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public) ?? throw new Exception("Method not found");
        return methodInfo.Invoke(null, [queryable]) as IQueryable<T> ?? throw new Exception("Query method invocation returned null");
    }
}
