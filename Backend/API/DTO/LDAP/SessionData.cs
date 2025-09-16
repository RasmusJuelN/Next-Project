using Novell.Directory.Ldap;

namespace API.DTO.LDAP;

public class SessionData
{
    public required LdapConnection Connection { get; set; }
    public byte[]? Cookie { get; set; }
}
