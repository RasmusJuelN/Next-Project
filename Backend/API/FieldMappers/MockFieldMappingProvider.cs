using API.DTO.User;
using API.Interfaces;
using Database.Enums;

namespace API.FieldMappers;

public class MockFieldMappingProvider : IFieldMappingProvider
{
    private static readonly Dictionary<Type, Dictionary<string, string>> _mappings = new()
    {
        [typeof(BasicUserInfo)] = new()
        {
            [nameof(BasicUserInfo.Name)] = "FullName",
            [nameof(BasicUserInfo.Username)] = "Username",
            [nameof(BasicUserInfo.MemberOf)] = "Role"
        },
        [typeof(BasicUserInfoWithUserID)] = new()
        {
            [nameof(BasicUserInfoWithUserID.UserId)] = "Id",
            [nameof(BasicUserInfoWithUserID.Name)] = "FullName",
            [nameof(BasicUserInfoWithUserID.Username)] = "Username",
            [nameof(BasicUserInfoWithUserID.MemberOf)] = "Role"
        },
        [typeof(BasicGroupInfo)] = new()
        {
            [nameof(BasicGroupInfo.GroupName)] = "Role"
        }
    };

    private static readonly Dictionary<string, Func<object, Type, object>> _converters = new()
    {
        ["Id"] = (obj, type) =>
        {
            if (obj is string StringId)
            {
                if (type == typeof(Guid))
                {
                    return new Guid(StringId);
                }
                else if (type == typeof(string))
                {
                    return new Guid(StringId).ToString();
                }
            }
            else if (obj is Guid GuidId)
            {
                if (type == typeof(Guid))
                {
                    return GuidId;
                }
                else if (type == typeof(string))
                {
                    return GuidId.ToString();
                }
            }
            throw new InvalidCastException("Expected string or Guid for Id conversion.");
        },
        ["FullName"] = (obj, type) =>
        {
            if (obj is string name)
            {
                return name;
            }
            throw new InvalidCastException("Expected string for Name conversion.");
        },
        ["Username"] = (obj, type) =>
        {
            if (obj is string username)
            {
                return username;
            }
            throw new InvalidCastException("Expected string for Username conversion.");
        },
        ["Role"] = (obj, type) =>
        {
            if (obj is UserRoles role)
            {
                if (type == typeof(List<string>))
                {
                    return new List<string> { role.ToString() };
                }
                else if (type == typeof(string))
                {
                    return role.ToString();
                }
            }
            throw new InvalidCastException("Expected string for Role conversion.");
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
