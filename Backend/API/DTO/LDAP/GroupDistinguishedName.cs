using API.Attributes;
using Novell.Directory.Ldap;

namespace API.DTO.LDAP;

/// <summary>
/// Represents a group distinguished name in LDAP, mapping the <c>distinguishedName</c> attribute.
/// </summary>
public record class GroupDistinguishedName
{
    /// <summary>
    /// Gets or sets the LDAP attribute for the group's distinguished name.
    /// </summary>
    [AuthenticationMapping("distinguishedName")]
    public LdapAttribute DistinguishedName { get; set; } = new LdapAttribute("distinguishedName");
}
// i try to add this to get all student from individual class group
public class LdapUserDTO
{
    [AuthenticationMapping("cn")]
    public string Name { get; set; }
    [AuthenticationMapping("memberOf")]
    public string ClassName { get; set; } = string.Empty;
}
