using API.Attributes;
using Novell.Directory.Ldap;

namespace API.DTO.LDAP;

public record class BasicUserInfo
{
    [LDAPMapping("sAMAccountName")]
    public LdapAttribute Username { get; set; } = new LdapAttribute("sAMAccountName");
    
    [LDAPMapping("name")]
    public LdapAttribute Name { get; set; } = new LdapAttribute("name");

    [LDAPMapping("memberOf")]
    public LdapAttribute MemberOf { get; set; } = new LdapAttribute("memberOf");
}
