using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using API.Attributes;

namespace API.Linq;

public class LdapQueryTranslator : ExpressionVisitor
{
    private readonly ILogger<LdapQueryTranslator> _logger;
    private StringBuilder _ldapFilter = new();

    public LdapQueryTranslator()
    {
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<LdapQueryTranslator>();
    }

    public string Translate(Expression expression)
    {
        _logger.LogDebug("Starting LDAP query translation for expression: {ExpressionType}", expression.Type.Name);
        
        _ldapFilter.Clear();
        Visit(expression);
        
        var result = _ldapFilter.ToString();
        _logger.LogDebug("Completed LDAP query translation. Filter: {LdapFilter}", result);
        
        return result;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        _logger.LogDebug("Visiting method call: {MethodName} on {DeclaringType}", node.Method.Name, node.Method.DeclaringType?.Name ?? "Unknown");
        
        if (node.Method.Name == "Where")
        {
            _logger.LogDebug("Processing Where clause");
            // Don't add extra parentheses - let the inner conditions handle their own parentheses
            Visit(node.Arguments[1]);
            return node;
        }
        else if (node.Method.Name == "Contains" && node.Method.DeclaringType == typeof(string))
        {
            _logger.LogDebug("Processing string Contains method");
            _ldapFilter.Append('(');
            Visit(node.Object);
            _ldapFilter.Append("=*");
            
            var searchValue = ExtractValue(node.Arguments[0]);
            _logger.LogDebug("Extracted Contains search value: {SearchValue}", searchValue);
            _ldapFilter.Append(searchValue);
            
            _ldapFilter.Append("*)");
            return node;
        }

        _logger.LogDebug("Method call not handled, delegating to base implementation");
        return base.VisitMethodCall(node);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        _logger.LogDebug("Visiting binary expression: {NodeType}", node.NodeType);
        
        switch (node.NodeType)
        {
            case ExpressionType.Equal:
                _logger.LogDebug("Processing equality comparison");
                _ldapFilter.Append('(');
                Visit(node.Left);
                _ldapFilter.Append('=');
                
                // Extract value for the right side of equality
                if (node.Right is ConstantExpression || 
                    (node.Right is MemberExpression memberExpr && memberExpr.Expression is ConstantExpression))
                {
                    var value = ExtractValue(node.Right);
                    _logger.LogDebug("Extracted equality comparison value: {Value}", value);
                    _ldapFilter.Append(value?.ToString() ?? "");
                }
                else
                {
                    Visit(node.Right);
                }
                _ldapFilter.Append(')');
                break;

            case ExpressionType.AndAlso:
                _logger.LogDebug("Processing AND operation");
                _ldapFilter.Append("(&");
                Visit(node.Left);
                Visit(node.Right);
                _ldapFilter.Append(')');
                break;

            case ExpressionType.OrElse:
                _logger.LogDebug("Processing OR operation");
                _ldapFilter.Append("(|");
                Visit(node.Left);
                Visit(node.Right);
                _ldapFilter.Append(')');
                break;
                
            default:
                _logger.LogWarning("Unsupported binary expression type: {NodeType}", node.NodeType);
                break;
        }

        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        _logger.LogDebug("Visiting member expression: {MemberName} on {DeclaringType}", node.Member.Name, node.Member.DeclaringType?.Name ?? "Unknown");
        
        var mappingAttr = node.Member.GetCustomAttribute<AuthenticationMapping>();
        var entryName = mappingAttr?.EntryName ?? node.Member.Name;
        
        _logger.LogDebug("Mapping member {MemberName} to LDAP attribute {LdapAttribute}", node.Member.Name, entryName);
        _ldapFilter.Append(entryName);
        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        var value = node.Value?.ToString() ?? "";
        _logger.LogDebug("Visiting constant expression with value: {Value}", value);
        
        _ldapFilter.Append(value);
        return node;
    }

    private object? ExtractValue(Expression expression)
    {
        _logger.LogDebug("Extracting value from expression: {ExpressionType}", expression.GetType().Name);
        
        switch (expression)
        {
            case ConstantExpression constant:
                _logger.LogDebug("Extracted constant value: {Value}", constant.Value);
                return constant.Value;
                
            case MemberExpression member when member.Expression is ConstantExpression memberConstant:
                // Handle cases like: var searchTerm = "john"; user.Name.Contains(searchTerm)
                var container = memberConstant.Value;
                var field = member.Member;
                
                object? value = field switch
                {
                    System.Reflection.FieldInfo fieldInfo => fieldInfo.GetValue(container),
                    System.Reflection.PropertyInfo propertyInfo => propertyInfo.GetValue(container),
                    _ => null
                };
                
                _logger.LogDebug("Extracted member value: {Value} from {MemberName}", value, member.Member.Name);
                return value;
                
            default:
                try
                {
                    // For more complex expressions, compile and execute them
                    var compiled = Expression.Lambda(expression).Compile();
                    var result = compiled.DynamicInvoke();
                    _logger.LogDebug("Extracted compiled expression value: {Value}", result);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not extract value from expression: {Expression}", expression);
                    return null;
                }
        }
    }
}
