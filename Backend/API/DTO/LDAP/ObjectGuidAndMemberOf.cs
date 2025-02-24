using API.Attributes;
using Novell.Directory.Ldap;

namespace API.DTO.LDAP;

public class ObjectGuidAndMemberOf
{
    [LDAPMapping("objectGUID")]
    public LdapAttribute ObjectGUID { get; set; } = new LdapAttribute("objectGUID");
    [LDAPMapping("memberOf")]
    public LdapAttribute MemberOf { get; set; } = new LdapAttribute("memberOf");
    [LDAPMapping("name")]
    public LdapAttribute Name { get; set; } = new LdapAttribute("name");
}
