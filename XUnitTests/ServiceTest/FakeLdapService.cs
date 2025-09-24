using API.DTO.LDAP;
using API.Services;
using Microsoft.Extensions.Configuration;

public class FakeLdapService : LdapService
{

    public FakeLdapService(LdapSessionCacheService ldapSessionCache, IConfiguration configuration)
        : base(configuration, ldapSessionCache)
    { }

    public new void Authenticate(string username, string password) { }
    public new void Authenticate() { }
    public new List<LdapUserDTO> GetStudentsInGroup(string groupName) => new List<LdapUserDTO>();
    public new List<string> GetAllGroups() => new List<string> { "TestGroup1", "TestGroup2" };
    public new string GetBaseDN() => "DC=fake,DC=local";
}