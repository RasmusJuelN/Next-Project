using API.DTO.LDAP;
using API.DTO.Responses.User;

namespace API.Extensions;

/// <summary>
/// Provides extension methods for converting <see cref="BasicUserInfoWithObjectGuid"/> objects to other data transfer objects.
/// </summary>
public static class BasicUserInfoWithObjectGuidExtensions
{
    /// <summary>
    /// Converts a <see cref="BasicUserInfoWithObjectGuid"/> instance to a <see cref="LdapUserBase"/> data transfer object.
    /// </summary>
    /// <param name="user">The <see cref="BasicUserInfoWithObjectGuid"/> instance to convert.</param>
    /// <returns>A new <see cref="LdapUserBase"/> instance populated with data from the input user object.</returns>
    /// <remarks>
    /// This method maps the following properties:
    /// <list type="bullet">
    /// <item><description>ObjectGUID.ByteValue to Id</description></item>
    /// <item><description>Name.StringValue to FullName</description></item>
    /// <item><description>Username.StringValue to UserName</description></item>
    /// </list>
    /// </remarks>
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
