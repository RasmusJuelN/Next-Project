namespace API.Attributes;

/// <summary>
/// Specifies the LDAP attribute name that a field or property is mapped to.
/// </summary>
/// <param name="name">The name of the LDAP attribute.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class LDAPMapping(string name) : Attribute
{
    public string Name = name;
}
