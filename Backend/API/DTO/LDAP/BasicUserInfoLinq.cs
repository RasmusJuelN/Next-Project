using API.Attributes;
using Novell.Directory.Ldap;

namespace API.DTO.LDAP;

/// <summary>
/// Represents basic user information retrieved from LDAP.
/// </summary>
public record class BasicUserInfoLinq
{
    /// <summary>
    /// Gets or sets the LDAP attribute for the user's account name (sAMAccountName).
    /// </summary>
    [AuthenticationMapping("sAMAccountName")]
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the LDAP attribute for the user's display name (name).
    /// </summary>
    [AuthenticationMapping("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the LDAP attribute for the user's group memberships (memberOf).
    /// </summary>
    [AuthenticationMapping("memberOf")]
    public required string MemberOf { get; set; }
}
