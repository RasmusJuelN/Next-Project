using System.Linq.Expressions;
using API.Services;
using API.Services.Authentication;
using Settings.Models;

namespace API.Linq;

public class LdapQueryProvider(ActiveDirectoryAuthenticationBridge authBridge, ILogger logger, IConfiguration configuration) : IQueryProvider
{
    private readonly ActiveDirectoryAuthenticationBridge _authBridge = authBridge;
    private readonly ILogger _logger = logger;
    private readonly LDAPSettings _ldapSettings = ConfigurationBinderService.Bind<LDAPSettings>(configuration);

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

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        _logger.LogDebug("Creating generic query for type: {ElementType}, expression type: {ExpressionType}", 
            typeof(TElement).Name, expression.Type.Name);
        
        var result = new LdapQueryable<TElement>(this, expression, _logger);
        _logger.LogDebug("Successfully created LdapQueryable<{ElementType}>", typeof(TElement).Name);
        
        return result;
    }

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
