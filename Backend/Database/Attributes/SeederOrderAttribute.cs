namespace Database.Attributes;

/// <summary>
/// Specifies the execution order for data seeders. Seeders with lower order values execute first.
/// </summary>
/// <remarks>
/// Initializes a new instance of the SeederOrderAttribute with the specified order.
/// </remarks>
/// <param name="order">The execution order. Lower values execute first. Default is 0.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SeederOrderAttribute(int order = 0) : Attribute
{
    public int Order { get; } = order;
}