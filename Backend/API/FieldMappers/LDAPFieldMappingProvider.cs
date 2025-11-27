namespace API.FieldMappers;

public class LDAPFieldMappingProvider : IFieldMappingProvider
{
    private static readonly Dictionary<Type, Dictionary<string, string>> _mappings = new()
    {
        [typeof(BasicUserInfo)] = new()
        {
            [nameof(BasicUserInfo.Name)] = "name",
            [nameof(BasicUserInfo.Username)] = "sAMAccountName",
            [nameof(BasicUserInfo.MemberOf)] = "memberOf"
        },
        [typeof(BasicUserInfoWithUserID)] = new()
        {
            [nameof(BasicUserInfoWithUserID.UserId)] = "objectGUID",
            [nameof(BasicUserInfoWithUserID.Name)] = "name",
            [nameof(BasicUserInfoWithUserID.Username)] = "sAMAccountName",
            [nameof(BasicUserInfoWithUserID.MemberOf)] = "memberOf"
        },
        [typeof(BasicGroupInfo)] = new()
        {
            [nameof(BasicGroupInfo.GroupName)] = "distinguishedName"
        }
    };

    private static readonly Dictionary<string, Func<object, Type, object>> _converters = new()
    {
        ["objectGUID"] = (obj, type) =>
        {
            if (obj is LdapAttribute objectGUID)
            {
                if (type == typeof(Guid))
                {
                    return new Guid(objectGUID.ByteValue);
                }
                else if (type == typeof(string))
                {
                    return new Guid(objectGUID.ByteValue).ToString();
                }
            }
            throw new InvalidCastException("Expected LdapAttribute for objectGUID conversion.");
        },
        ["name"] = (obj, type) =>
        {
            if (obj is LdapAttribute name)
            {
                return name.StringValue;
            }
            throw new InvalidCastException("Expected LdapAttribute for name conversion.");
        },
        ["sAMAccountName"] = (obj, type) =>
        {
            if (obj is LdapAttribute sAMAccountName)
            {
                return sAMAccountName.StringValue;
            }
            throw new InvalidCastException("Expected LdapAttribute for sAMAccountName conversion.");
        },
        ["memberOf"] = (obj, type) =>
        {
            if (obj is LdapAttribute memberOf)
            {
                return memberOf.StringValue.Split(';').ToList();
            }
            throw new InvalidCastException("Expected LdapAttribute for memberOf conversion.");
        },
        ["distinguishedName"] = (obj, type) =>
        {
            if (obj is LdapAttribute distinguishedName)
            {
                return distinguishedName.StringValue;
            }
            throw new InvalidCastException("Expected LdapAttribute for distinguishedName conversion.");
        }
    };

    public Dictionary<string, string> GetFieldMappings<TModel>()
    {
        return _mappings.TryGetValue(typeof(TModel), out var mappings)
            ? mappings : [];
    }

    public Dictionary<string, Func<object, Type, object>> GetFieldConverter()
    {
        return _converters;
    }
}
