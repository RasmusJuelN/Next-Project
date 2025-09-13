using System.Collections;
using System.Linq.Expressions;

namespace API.Linq;

/// <summary>
/// Represents a queryable collection that translates LINQ operations into LDAP search operations.
/// This class implements <see cref="IQueryable{T}"/> to provide LINQ query capabilities over LDAP data sources.
/// </summary>
/// <typeparam name="T">The type of objects in the queryable collection. This should typically be an LDAP DTO class.</typeparam>
/// <remarks>
/// <para>The <see cref="LdapQueryable{T}"/> class serves as the entry point for LINQ-to-LDAP operations.
/// It works in conjunction with <see cref="LdapQueryProvider"/> and <see cref="LdapQueryTranslator"/>
/// to convert LINQ expressions into LDAP search filters and execute them against an LDAP directory.</para>
/// 
/// <para><strong>Usage Pattern:</strong></para>
/// <list type="number">
/// <item><description>LINQ expressions are built against the queryable</description></item>
/// <item><description>When enumerated, the expressions are passed to the query provider</description></item>
/// <item><description>The provider translates expressions to LDAP filters</description></item>
/// <item><description>LDAP searches are executed and results are returned</description></item>
/// </list>
/// 
/// <para><strong>Supported LINQ Operations:</strong></para>
/// <list type="bullet">
/// <item><description>Where() - Filtering with complex predicates</description></item>
/// <item><description>FirstOrDefault() - Single result retrieval</description></item>
/// <item><description>Property equality and string Contains operations</description></item>
/// <item><description>Logical AND/OR operations</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// var queryable = new LdapQueryable&lt;User&gt;(provider, logger);
/// var results = queryable
///     .Where(u =&gt; u.Name.Contains("john") &amp;&amp; u.Email != null)
///     .ToList();
/// </code>
/// </example>
public class LdapQueryable<T> : IQueryable<T>
{
    private readonly ILogger<LdapQueryable<T>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LdapQueryable{T}"/> class with a constant expression.
    /// This constructor is typically used for the root queryable instance.
    /// </summary>
    /// <param name="provider">The query provider that will handle expression translation and execution</param>
    /// <param name="logger">Logger instance for debugging and monitoring query operations</param>
    public LdapQueryable(IQueryProvider provider, ILogger<LdapQueryable<T>> logger)
    {
        _logger = logger;
        
        Provider = provider;
        Expression = Expression.Constant(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LdapQueryable{T}"/> class with a specific expression.
    /// This constructor is used when creating derived queryables from LINQ operations.
    /// </summary>
    /// <param name="provider">The query provider that will handle expression translation and execution</param>
    /// <param name="expression">The expression tree representing the query operations</param>
    /// <param name="logger">Logger instance for debugging and monitoring query operations</param>
    public LdapQueryable(IQueryProvider provider, Expression expression, ILogger<LdapQueryable<T>> logger)
    {
        _logger = logger;
        
        Provider = provider;
        Expression = expression;
    }

    /// <summary>
    /// Gets the type of the elements in the queryable collection.
    /// </summary>
    /// <value>The <see cref="Type"/> representing <typeparamref name="T"/>.</value>
    public Type ElementType => typeof(T);
    
    /// <summary>
    /// Gets the expression tree that represents the current query.
    /// </summary>
    /// <value>An <see cref="Expression"/> that represents the current query operations.</value>
    public Expression Expression { get; }
    
    /// <summary>
    /// Gets the query provider that will execute the query represented by this instance.
    /// </summary>
    /// <value>An <see cref="IQueryProvider"/> that can execute LDAP queries.</value>
    public IQueryProvider Provider { get; }

    /// <summary>
    /// Returns an enumerator that iterates through the collection by executing the LDAP query.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{T}"/> that can be used to iterate through the query results.</returns>
    /// <remarks>
    /// This method triggers the actual execution of the LDAP query. The expression tree is passed
    /// to the query provider, which translates it to an LDAP filter and executes the search.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when the LDAP query execution fails</exception>
    public IEnumerator<T> GetEnumerator()
    {
        _logger.LogDebug("Getting enumerator for LdapQueryable<{Type}>", typeof(T).Name);
        
        try
        {
            var result = Provider.Execute<IEnumerable<T>>(Expression);
            _logger.LogDebug("Successfully executed query for LdapQueryable<{Type}>", typeof(T).Name);
            return result.GetEnumerator();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query for LdapQueryable<{Type}>", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Returns a non-generic enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> that can be used to iterate through the query results.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        _logger.LogDebug("Getting non-generic enumerator for LdapQueryable<{Type}>", typeof(T).Name);
        return GetEnumerator();
    }
}
