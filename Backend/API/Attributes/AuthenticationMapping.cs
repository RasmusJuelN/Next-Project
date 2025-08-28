namespace API.Attributes;

/// <summary>
/// Attribute that maps a target field or property to a named authentication entry.
/// </summary>
/// <remarks>
/// Apply to fields or properties to associate them with a specific authentication-related entry
/// (for example, a configuration value, claim, or context item) identified by name.
/// Targets: Field, Property.
/// </remarks>
/// <param name="entryName">
/// The key/name of the authentication entry to map the annotated member to.
/// </param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class AuthenticationMapping(string entryName) : Attribute
{
    public string EntryName = entryName;
}