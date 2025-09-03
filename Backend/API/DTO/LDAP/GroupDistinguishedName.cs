using API.Attributes;
using Novell.Directory.Ldap;

namespace API.DTO.LDAP;

public record class GroupDistinguishedName
{
    [LDAPMapping("distinguishedName")]
    public LdapAttribute DistinguishedName { get; set; } = new LdapAttribute("distinguishedName");
}
// i try to add this to get all student from individual class group
public class LdapUserDTO
{
    [LDAPMapping("cn")]
    public string Name { get; set; }
}
