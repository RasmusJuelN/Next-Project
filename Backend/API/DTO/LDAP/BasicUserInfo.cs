using API.Attributes;
using Novell.Directory.Ldap;

namespace API.DTO.LDAP;

/// <summary>
/// Represents basic user information retrieved from LDAP.
/// </summary>
public record class BasicUserInfo
{
    /// <summary>
    /// Gets or sets the LDAP attribute for the user's account name (sAMAccountName).
    /// </summary>
    [LDAPMapping("sAMAccountName")]
    public LdapAttribute Username { get; set; } = new LdapAttribute("sAMAccountName");

    /// <summary>
    /// Gets or sets the LDAP attribute for the user's display name (name).
    /// </summary>
    [LDAPMapping("name")]
    public LdapAttribute Name { get; set; } = new LdapAttribute("name");

    /// <summary>
    /// Gets or sets the LDAP attribute for the user's group memberships (memberOf).
    /// </summary>
    [LDAPMapping("memberOf")]
    public LdapAttribute MemberOf { get; set; } = new LdapAttribute("memberOf");
}
