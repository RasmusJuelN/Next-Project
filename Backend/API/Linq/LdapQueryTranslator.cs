using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using API.Attributes;

namespace API.Linq;

/// <summary>
/// Translates .NET LINQ expressions into LDAP search filters by visiting expression trees.
/// This class implements the Visitor pattern to traverse expression nodes and build corresponding LDAP filter syntax.
/// </summary>
/// <remarks>
/// The translator supports common LINQ operations and converts them to LDAP filter format:
/// <list type="bullet">
/// <item><description>Equality comparisons (==) → (attribute=value)</description></item>
/// <item><description>String.Contains() → (attribute=*value*)</description></item>
/// <item><description>Logical AND (&amp;&amp;) → (&amp;(condition1)(condition2))</description></item>
/// <item><description>Logical OR (||) → (|(condition1)(condition2))</description></item>
/// <item><description>Property mappings via AuthenticationMapping attributes</description></item>
/// </list>
/// 
/// <para><strong>LDAP Filter Syntax Examples:</strong></para>
/// <list type="table">
/// <listheader>
/// <term>LINQ Expression</term>
/// <description>LDAP Filter</description>
/// </listheader>
/// <item>
/// <term>u =&gt; u.Name == "john"</term>
/// <description>(Name=john)</description>
/// </item>
/// <item>
/// <term>u =&gt; u.Name.Contains("joh")</term>
/// <description>(Name=*joh*)</description>
/// </item>
/// <item>
/// <term>u =&gt; u.Name == "john" &amp;&amp; u.Email == "john@test.com"</term>
/// <description>(&amp;(Name=john)(mail=john@test.com))</description>
/// </item>
/// <item>
/// <term>u =&gt; u.Name == "john" || u.Name == "jane"</term>
/// <description>(|(Name=john)(Name=jane))</description>
/// </item>
/// </list>
/// 
/// <para><strong>Attribute Mapping:</strong></para>
/// Properties decorated with <see cref="AuthenticationMapping"/> attributes are automatically
/// mapped to their corresponding LDAP attribute names during translation.
/// </remarks>
/// <param name="logger">Logger instance for debugging and monitoring translation operations</param>
/// <example>
/// <code>
/// var translator = new LdapQueryTranslator(logger);
/// Expression&lt;Func&lt;User, bool&gt;&gt; expr = u =&gt; u.Name == "john" &amp;&amp; u.Email.Contains("test");
/// string ldapFilter = translator.Translate(expr.Body);
/// Result: (&amp;(Name=john)(mail=*test*))
/// </code>
/// </example>
public class LdapQueryTranslator(ILogger<LdapQueryTranslator> logger) : ExpressionVisitor
{
    private readonly ILogger<LdapQueryTranslator> _logger = logger;
    private readonly StringBuilder _ldapFilter = new();

    /// <summary>
    /// Translates a LINQ expression tree into an LDAP search filter string.
    /// </summary>
    /// <param name="expression">The root expression to translate. This should be the body of a lambda expression.</param>
    /// <returns>A valid LDAP search filter string that represents the given expression.</returns>
    /// <remarks>
    /// This method clears any previous filter state and processes the entire expression tree,
    /// building the LDAP filter incrementally by visiting each node in the expression.
    /// The resulting filter follows RFC 4515 LDAP search filter syntax.
    /// </remarks>
    /// <example>
    /// <code>
    /// Expression&lt;Func&lt;User, bool&gt;&gt; expr = u => u.Name == "john";
    /// string filter = translator.Translate(expr.Body);
    /// // Returns: "(Name=john)"
    /// </code>
    /// </example>
    public string Translate(Expression expression)
    {
        _logger.LogDebug("Starting LDAP query translation for expression: {ExpressionType}", expression.Type.Name);
        
        _ldapFilter.Clear();
        Visit(expression);
        
        var result = _ldapFilter.ToString();
        _logger.LogDebug("Completed LDAP query translation. Filter: {LdapFilter}", result);
        
        return result;
    }

    /// <summary>
    /// Visits method call expressions and translates supported LINQ methods to LDAP filter operations.
    /// </summary>
    /// <param name="node">The method call expression node to process</param>
    /// <returns>The processed expression node</returns>
    /// <remarks>
    /// <para><strong>Supported Methods:</strong></para>
    /// <list type="bullet">
    /// <item><description><strong>Where:</strong> Processes the predicate expression within the Where clause</description></item>
    /// <item><description><strong>String.Contains:</strong> Translates to LDAP wildcard syntax (attribute=*value*)</description></item>
    /// <item><description><strong>FirstOrDefault:</strong> Processes the source query while ignoring the aggregation</description></item>
    /// </list>
    /// 
    /// <para>Unsupported method calls are delegated to the base ExpressionVisitor implementation.</para>
    /// </remarks>
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
        else if (node.Method.Name == "FirstOrDefault" && node.Method.DeclaringType?.Name == "Queryable")
        {
            _logger.LogDebug("Processing FirstOrDefault method - processing source query");
            Visit(node.Arguments[0]);
            return node;
        }

        _logger.LogDebug("Method call not handled, delegating to base implementation");
        return base.VisitMethodCall(node);
    }

    /// <summary>
    /// Visits binary expressions and translates logical operations to LDAP filter syntax.
    /// </summary>
    /// <param name="node">The binary expression node to process</param>
    /// <returns>The processed expression node</returns>
    /// <remarks>
    /// <para><strong>Supported Binary Operations:</strong></para>
    /// <list type="bullet">
    /// <item><description><strong>Equal:</strong> Translates to (attribute=value)</description></item>
    /// <item><description><strong>AndAlso:</strong> Translates to (&amp;(left)(right))</description></item>
    /// <item><description><strong>OrElse:</strong> Translates to (|(left)(right))</description></item>
    /// </list>
    /// 
    /// <para>The method handles value extraction from constant expressions and member expressions 
    /// to properly construct the LDAP filter values.</para>
    /// </remarks>
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
            
            case ExpressionType.NotEqual:
                _logger.LogDebug("Processing NOT EQUAL operation");

                //If the value is null, use presence check instead of not equal operator
                if (node.Right is ConstantExpression constExpr && constExpr.Value == null)
                {
                    _ldapFilter.Append('(');
                    Visit(node.Left);
                    _ldapFilter.Append("=*)");
                }
                else
                {
                    _ldapFilter.Append("(!(");
                    Visit(node.Left);
                    _ldapFilter.Append('=');
                    
                    var value = ExtractValue(node.Right);
                    _logger.LogDebug("Extracted not equal comparison value: {Value}", value);
                    _ldapFilter.Append(value?.ToString() ?? "");
                    
                    _ldapFilter.Append("))");
                }
                break;
            
                
            default:
                _logger.LogWarning("Unsupported binary expression type: {NodeType}", node.NodeType);
                break;
        }

        return node;
    }

    /// <summary>
    /// Visits member expressions and maps property names to their corresponding LDAP attributes.
    /// </summary>
    /// <param name="node">The member expression node representing a property access</param>
    /// <returns>The processed expression node</returns>
    /// <remarks>
    /// This method checks for <see cref="AuthenticationMapping"/> attributes on properties
    /// and uses the mapped LDAP attribute name if present. If no mapping is found,
    /// it uses the property name directly.
    /// 
    /// <para><strong>Example:</strong></para>
    /// If a property <c>CommonName</c> has <c>[AuthenticationMapping("cn")]</c>,
    /// the method will output "cn" instead of "CommonName" in the LDAP filter.
    /// </remarks>
    protected override Expression VisitMember(MemberExpression node)
    {
        _logger.LogDebug("Visiting member expression: {MemberName} on {DeclaringType}", node.Member.Name, node.Member.DeclaringType?.Name ?? "Unknown");
        
        var mappingAttr = node.Member.GetCustomAttribute<AuthenticationMapping>();
        var entryName = mappingAttr?.EntryName ?? node.Member.Name;
        
        _logger.LogDebug("Mapping member {MemberName} to LDAP attribute {LdapAttribute}", node.Member.Name, entryName);
        _ldapFilter.Append(entryName);
        return node;
    }

    /// <summary>
    /// Visits constant expressions and appends their string values to the LDAP filter.
    /// </summary>
    /// <param name="node">The constant expression node containing a literal value</param>
    /// <returns>The processed expression node</returns>
    /// <remarks>
    /// This method converts constant values to their string representation for use in LDAP filters.
    /// Null values are converted to empty strings.
    /// </remarks>
    protected override Expression VisitConstant(ConstantExpression node)
    {
        var value = node.Value?.ToString() ?? "";
        _logger.LogDebug("Visiting constant expression with value: {Value}", value);
        
        _ldapFilter.Append(value);
        return node;
    }

    /// <summary>
    /// Extracts values from complex expressions, including variable references and compiled expressions.
    /// </summary>
    /// <param name="expression">The expression from which to extract a value</param>
    /// <returns>The extracted value as an object, or null if extraction fails</returns>
    /// <remarks>
    /// <para>This method handles several types of value extraction:</para>
    /// <list type="bullet">
    /// <item><description><strong>Constant expressions:</strong> Direct value extraction</description></item>
    /// <item><description><strong>Member expressions:</strong> Field/property value extraction from closure variables</description></item>
    /// <item><description><strong>Complex expressions:</strong> Compilation and execution for dynamic values</description></item>
    /// </list>
    /// 
    /// <para>This is particularly useful for handling lambda expressions that reference local variables,
    /// such as: <c>var name = "john"; users.Where(u =&gt; u.Name == name)</c></para>
    /// </remarks>
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
