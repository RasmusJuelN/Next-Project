using API.FieldMappers;

namespace UnitTests.API;

public class LDAPFieldMappingProviderTests
{
    [Fact]
    public void TestAllMappingsHaveAConverter()
    {
        LDAPFieldMappingProvider provider = new();
        Dictionary<string, Func<object, Type, object>> converters = provider.GetFieldConverter();

        foreach (var mapping in typeof(LDAPFieldMappingProvider)
                     .GetField("_mappings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                     ?.GetValue(null) as Dictionary<Type, Dictionary<string, string>> ?? [])
        {
            foreach (var ldapAttribute in mapping.Value.Values)
            {
                Assert.True(
                    converters.ContainsKey(ldapAttribute),
                    $"No converter found for LDAP attribute '{ldapAttribute}' in mapping for type '{mapping.Key.Name}'."
                );
            }
        }
    }
}
