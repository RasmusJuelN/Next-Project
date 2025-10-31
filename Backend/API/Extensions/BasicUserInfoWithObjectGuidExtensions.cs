using API.DTO.LDAP;
using API.DTO.Responses.User;

namespace API.Extensions;

/// <summary>
/// Provides extension methods for converting <see cref="BasicUserInfoWithUserID"/> objects to other data transfer objects.
/// </summary>
public static class BasicUserInfoWithObjectGuidExtensions
{
    /// <summary>
    /// Converts a <see cref="BasicUserInfoWithUserID"/> instance to a <see cref="LdapUserBase"/> data transfer object.
    /// </summary>
    /// <param name="user">The <see cref="BasicUserInfoWithUserID"/> instance to convert.</param>
    /// <returns>A new <see cref="LdapUserBase"/> instance populated with data from the input user object.</returns>
    /// <remarks>
    /// This method maps the following properties:
    /// <list type="bullet">
    /// <item><description>ObjectGUID.ByteValue to Id</description></item>
    /// <item><description>Name.StringValue to FullName</description></item>
    /// <item><description>Username.StringValue to UserName</description></item>
    /// </list>
    /// </remarks>
    public static LdapUserBase ToUserBaseDto(this BasicUserInfoWithUserID user)
    {
        return new()
        {
            Id = new(user.UserId),
            FullName = user.Name,
            UserName = user.Username
        };
    }
}
