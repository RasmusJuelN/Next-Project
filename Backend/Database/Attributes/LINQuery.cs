namespace Database.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class QueryMethodAttribute(string queryMethod) : Attribute
{
    public string QueryMethod = queryMethod;
}
