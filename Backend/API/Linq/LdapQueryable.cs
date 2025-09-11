using System;
using System.Collections;
using System.Linq.Expressions;

namespace API.Linq;

public class LdapQueryable<T> : IQueryable<T>
{
    private readonly ILogger _logger;

    public LdapQueryable(IQueryProvider provider, ILogger logger)
    {
        _logger = logger;
        
        Provider = provider;
        Expression = Expression.Constant(this);
    }

    public LdapQueryable(IQueryProvider provider, Expression expression, ILogger logger)
    {
        _logger = logger;
        
        Provider = provider;
        Expression = expression;
    }

    public Type ElementType => typeof(T);
    public Expression Expression { get; }
    public IQueryProvider Provider { get; }

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

    IEnumerator IEnumerable.GetEnumerator()
    {
        _logger.LogDebug("Getting non-generic enumerator for LdapQueryable<{Type}>", typeof(T).Name);
        return GetEnumerator();
    }
}
