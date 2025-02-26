using API.Attributes;
using Novell.Directory.Ldap;

namespace API.DTO.LDAP;

public record class BasicUserInfoWithObjectGuid : BasicUserInfo
{
    [LDAPMapping("objectGUID")]
    public LdapAttribute ObjectGUID { get; set; } = new LdapAttribute("objectGUID");
}
