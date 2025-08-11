using API.Attributes;
using Novell.Directory.Ldap;

namespace API.DTO.LDAP;

/// <summary>
/// Represents basic user information extended with the LDAP <c>objectGUID</c> attribute.
/// Inherits from <see cref="BasicUserInfo"/> and adds an <see cref="LdapAttribute"/> for the user's object GUID.
/// </summary>
public record class BasicUserInfoWithObjectGuid : BasicUserInfo
{
    [LDAPMapping("objectGUID")]
    public LdapAttribute ObjectGUID { get; set; } = new LdapAttribute("objectGUID");
}
