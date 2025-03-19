using API.DTO.LDAP;
using API.DTO.Responses.User;

namespace API.Extensions;

public static class BasicUserInfoWithObjectGuidExtensions
{
    public static LdapUserBase ToUserBaseDto(this BasicUserInfoWithObjectGuid user)
    {
        return new()
        {
            Id = new(user.ObjectGUID.ByteValue),
            FullName = user.Name.StringValue,
            UserName = user.Username.StringValue
        };
    }
}
