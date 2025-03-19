using API.Attributes;
using Novell.Directory.Ldap;

namespace API.DTO.LDAP;

public record class GroupDistinguishedName
{
    [LDAPMapping("distinguishedName")]
    public LdapAttribute DistinguishedName { get; set; } = new LdapAttribute("distinguishedName");
}
