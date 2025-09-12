using System.Linq.Expressions;
using API.Services;
using API.Services.Authentication;
using Settings.Models;

namespace API.Linq;

/// <summary>
/// Provides LINQ query execution capabilities for LDAP data sources by implementing the IQueryProvider interface.
/// This class bridges LINQ expression trees with LDAP search operations through the Active Directory authentication bridge.
/// </summary>
/// <remarks>
/// <para>The <see cref="LdapQueryProvider"/> is responsible for:</para>
/// <list type="bullet">
/// <item><description>Creating new <see cref="LdapQueryable{T}"/> instances for query composition</description></item>
/// <item><description>Translating LINQ expression trees to LDAP search filters</description></item>
/// <item><description>Executing LDAP searches via the authentication bridge</description></item>
/// <item><description>Handling both collection and single-item result scenarios</description></item>
/// </list>
/// 
/// <para><strong>Query Execution Flow:</strong></para>
/// <list type="number">
/// <item><description>Receives LINQ expression trees from queryable objects</description></item>
/// <item><description>Uses <see cref="LdapQueryTranslator"/> to convert expressions to LDAP filters</description></item>
/// <item><description>Invokes <see cref="ActiveDirectoryAuthenticationBridge.SearchLDAP{TLdapResult}"/> via reflection</description></item>
/// <item><description>Returns results as collections or single items based on query type</description></item>
/// </list>
/// 
/// <para><strong>Supported Result Types:</strong></para>
/// <list type="bullet">
/// <item><description><c>IEnumerable&lt;T&gt;</c> - Returns full result collections</description></item>
/// <item><description><c>T</c> - Returns single items (e.g., from FirstOrDefault)</description></item>
/// </list>
/// </remarks>
/// <param name="authBridge">The Active Directory authentication bridge for executing LDAP searches</param>
/// <param name="logger">Logger instance for debugging and monitoring query operations</param>
/// <param name="configuration">Configuration containing LDAP settings like base DN</param>
/// <example>
/// <code>
/// var provider = new LdapQueryProvider(authBridge, logger, configuration);
/// var queryable = new LdapQueryable&lt;User&gt;(provider, logger);
/// 
/// // This will be translated and executed as an LDAP search
/// var users = queryable.Where(u =&gt; u.Name == "john").ToList();
/// </code>
/// </example>
public class LdapQueryProvider(ActiveDirectoryAuthenticationBridge authBridge, ILogger logger, IConfiguration configuration) : IQueryProvider
{
    private readonly ActiveDirectoryAuthenticationBridge _authBridge = authBridge;
    private readonly ILogger _logger = logger;
    private readonly LDAPSettings _ldapSettings = ConfigurationBinderService.Bind<LDAPSettings>(configuration);

    /// <summary>
    /// Creates a new non-generic queryable object for the specified expression.
    /// </summary>
    /// <param name="expression">The expression tree representing the query</param>
    /// <returns>A new <see cref="IQueryable"/> instance that can execute LDAP queries</returns>
    /// <remarks>
    /// This method is called by LINQ operators to create new queryable instances.
    /// It dynamically determines the element type from the expression and creates
    /// the appropriate <see cref="LdapQueryable{T}"/> instance.
    /// </remarks>
    public IQueryable CreateQuery(Expression expression)
    {
        _logger.LogDebug("Creating non-generic query for expression type: {ExpressionType}", expression.Type.Name);
        
        var elementType = expression.Type.GetGenericArguments()[0];
        _logger.LogDebug("Extracted element type: {ElementType}", elementType.Name);
        
        var queryableType = typeof(LdapQueryable<>).MakeGenericType(elementType);
        var result = (IQueryable)Activator.CreateInstance(queryableType, this, expression)!;
        
        _logger.LogDebug("Successfully created LdapQueryable for type: {ElementType}", elementType.Name);
        return result;
    }

    /// <summary>
    /// Creates a new generic queryable object for the specified expression.
    /// </summary>
    /// <typeparam name="TElement">The type of elements in the queryable collection</typeparam>
    /// <param name="expression">The expression tree representing the query</param>
    /// <returns>A new <see cref="IQueryable{TElement}"/> instance that can execute LDAP queries</returns>
    /// <remarks>
    /// This is the generic version of <see cref="CreateQuery(Expression)"/> and is preferred
    /// when the element type is known at compile time.
    /// </remarks>
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        _logger.LogDebug("Creating generic query for type: {ElementType}, expression type: {ExpressionType}", 
            typeof(TElement).Name, expression.Type.Name);
        
        var result = new LdapQueryable<TElement>(this, expression, _logger);
        _logger.LogDebug("Successfully created LdapQueryable<{ElementType}>", typeof(TElement).Name);
        
        return result;
    }

    /// <summary>
    /// Executes a query represented by an expression tree and returns the result as an object.
    /// </summary>
    /// <param name="expression">The expression tree representing the query to execute</param>
    /// <returns>The result of the query execution as an object</returns>
    /// <remarks>
    /// This method handles non-generic query execution by dynamically determining the result type
    /// and using reflection to invoke the appropriate generic execution method.
    /// It's primarily used internally by the LINQ infrastructure.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when query execution fails</exception>
    public object Execute(Expression expression)
    {
        _logger.LogDebug("Executing non-generic query for expression type: {ExpressionType}", expression.Type.Name);
        
        LdapQueryTranslator translator = new(_logger);
        string ldapFilter = translator.Translate(expression);
        _logger.LogDebug("Generated LDAP filter: {LdapFilter}", ldapFilter);

        // For non-generic Execute, we need to handle the type dynamically
        var resultType = expression.Type;
        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            resultType = resultType.GetGenericArguments()[0];
            _logger.LogDebug("Extracted result type from IEnumerable: {ResultType}", resultType.Name);
        }

        _logger.LogDebug("Invoking ExecuteLdapQuery via reflection for type: {ResultType}", resultType.Name);
        
        try
        {
            var method = typeof(LdapQueryProvider).GetMethod(nameof(ExecuteLdapQuery), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var genericMethod = method!.MakeGenericMethod(resultType);
            var result = genericMethod.Invoke(this, new object[] { ldapFilter })!;
            
            _logger.LogDebug("Successfully executed non-generic query for type: {ResultType}", resultType.Name);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing non-generic query for type: {ResultType}", resultType.Name);
            throw;
        }
    }

    /// <summary>
    /// Executes a query represented by an expression tree and returns a strongly-typed result.
    /// </summary>
    /// <typeparam name="TResult">The expected type of the query result</typeparam>
    /// <param name="expression">The expression tree representing the query to execute</param>
    /// <returns>The result of the query execution, typed as <typeparamref name="TResult"/></returns>
    /// <remarks>
    /// <para>This method performs the core query execution logic:</para>
    /// <list type="number">
    /// <item><description>Translates the expression tree to an LDAP filter string</description></item>
    /// <item><description>Executes the LDAP search via <see cref="ExecuteLdapQuery{TResult}"/></description></item>
    /// <item><description>Returns results in the expected format</description></item>
    /// </list>
    /// 
    /// <para><strong>Result Type Handling:</strong></para>
    /// <list type="bullet">
    /// <item><description>For <c>IEnumerable&lt;T&gt;</c> results: Returns the full result collection</description></item>
    /// <item><description>For <c>T</c> results: Returns the first item or default value (e.g., FirstOrDefault scenarios)</description></item>
    /// </list>
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when LDAP query execution fails</exception>
    public TResult Execute<TResult>(Expression expression)
    {
        _logger.LogDebug("Executing generic query for type: {ResultType}, expression type: {ExpressionType}", 
            typeof(TResult).Name, expression.Type.Name);
        
        try
        {
            LdapQueryTranslator translator = new(_logger);
            string ldapFilter = translator.Translate(expression);
            _logger.LogDebug("Generated LDAP filter for generic execution: {LdapFilter}", ldapFilter);

            var result = ExecuteLdapQuery<TResult>(ldapFilter);
            _logger.LogDebug("Successfully executed generic query for type: {ResultType}", typeof(TResult).Name);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing generic query for type: {ResultType}", typeof(TResult).Name);
            throw;
        }
    }

    /// <summary>
    /// Executes an LDAP search operation using the provided filter and returns the results.
    /// </summary>
    /// <typeparam name="TResult">The expected type of the search result</typeparam>
    /// <param name="ldapFilter">The LDAP search filter string to execute</param>
    /// <returns>The search results, formatted according to the result type</returns>
    /// <remarks>
    /// <para>This method handles the low-level LDAP search execution:</para>
    /// <list type="number">
    /// <item><description>Determines whether the result should be a collection or single item</description></item>
    /// <item><description>Uses reflection to invoke the authentication bridge's SearchLDAP method</description></item>
    /// <item><description>Formats the results based on the expected return type</description></item>
    /// </list>
    /// 
    /// <para><strong>Result Type Processing:</strong></para>
    /// <list type="bullet">
    /// <item><description><strong>Collection results:</strong> <c>IEnumerable&lt;T&gt;</c> types return the full List&lt;T&gt;</description></item>
    /// <item><description><strong>Single item results:</strong> <c>T</c> types return the first item or default(T)</description></item>
    /// </list>
    /// 
    /// <para>The method uses the configured LDAP base DN and scope (LdapConnection.ScopeSub) for searches.</para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when the SearchLDAP method is not found on the authentication bridge</exception>
    /// <exception cref="Exception">Re-throws any exceptions from LDAP search operations</exception>
    private TResult ExecuteLdapQuery<TResult>(string ldapFilter)
    {
        _logger.LogDebug("Executing LDAP query with filter: {LdapFilter}", ldapFilter);
        
        string baseDN = _ldapSettings.BaseDN;
        _logger.LogDebug("Using base DN: {BaseDN}", baseDN);

        // Extract the actual entity type from IEnumerable<T> if needed
        Type resultType = typeof(TResult);
        Type entityType = resultType;
        bool isCollectionResult = false;
        
        _logger.LogDebug("Original result type: {ResultType}", resultType.Name);
        
        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            entityType = resultType.GetGenericArguments()[0];
            isCollectionResult = true;
            _logger.LogDebug("Extracted entity type from IEnumerable: {EntityType}", entityType.Name);
        }

        _logger.LogDebug("Invoking SearchLDAP method via reflection for entity type: {EntityType}", entityType.Name);
        
        try
        {
            var searchMethod = typeof(ActiveDirectoryAuthenticationBridge)
                .GetMethod("SearchLDAP", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?? throw new InvalidOperationException("SearchLDAP method not found on ActiveDirectoryAuthenticationBridge");
            
            var genericMethod = searchMethod.MakeGenericMethod(entityType);
            var results = genericMethod.Invoke(_authBridge, [ldapFilter, baseDN, 2]); // 2 = LdapConnection.ScopeSub
            
            _logger.LogDebug("LDAP query executed successfully, returning results for type: {ResultType}", typeof(TResult).Name);
            
            // If TResult is IEnumerable<T>, return the List<T> directly (it implements IEnumerable<T>)
            if (isCollectionResult)
            {
                return (TResult)results!;
            }
            
            // If TResult is T, return the first item from the list or default value
            var list = results as System.Collections.IList;
            if (list != null && list.Count > 0)
            {
                return (TResult)list[0]!;
            }
            
            return default(TResult)!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing LDAP query with filter: {LdapFilter} for entity type: {EntityType}", ldapFilter, entityType.Name);
            throw;
        }
    }
}
