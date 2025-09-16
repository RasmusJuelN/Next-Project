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
