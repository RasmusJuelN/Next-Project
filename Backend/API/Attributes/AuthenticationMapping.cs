namespace API.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class AuthenticationMapping(string entryName) : Attribute
{
    public string EntryName = entryName;
}