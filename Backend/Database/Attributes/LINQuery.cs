namespace Database.Attributes;

/// <summary>
/// Specifies the query method to be used for a field in LINQ operations.
/// This attribute can only be applied to fields.
/// </summary>
/// <param name="queryMethod">The name of the query method to associate with the decorated field.</param>
/// <remarks>
/// This attribute is used to define custom query behavior for fields in database operations.
/// The query method string should correspond to a valid query operation or method name
/// that will be used when processing LINQ queries for the decorated field.
/// </remarks>
/// <example>
/// <code>
/// [QueryMethod("Contains")]
/// private string searchField;
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Field)]
public class QueryMethodAttribute(string queryMethod) : Attribute
{
    public string QueryMethod = queryMethod;
}
