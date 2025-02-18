namespace API.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class LDAPMapping(string name) : Attribute
{
    public string Name = name;
}
