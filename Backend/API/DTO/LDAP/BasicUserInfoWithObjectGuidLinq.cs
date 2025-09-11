using API.Attributes;
using Novell.Directory.Ldap;

namespace API.DTO.LDAP;

/// <summary>
/// Represents basic user information extended with the LDAP <c>objectGUID</c> attribute.
/// Inherits from <see cref="BasicUserInfo"/> and adds an <see cref="LdapAttribute"/> for the user's object GUID.
/// </summary>
public record class BasicUserInfoWithObjectGuidLinq : BasicUserInfoLinq
{
    [AuthenticationMapping("objectGUID")]
    public required byte[] ObjectGUID { get; set; }
}
