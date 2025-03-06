using Database.DTO.User;
using Database.Models;

namespace Database.Extensions;

public static class UserBaseModelMapper
{
    public static UserBase ToBaseDto(this UserBaseModel user)
    {
        return new()
        {
            UserName = user.UserName,
            FullName = user.FullName
        };
    }

    public static User ToDto(this UserBaseModel user)
    {
        return new()
        {
            UserName = user.UserName,
            FullName = user.FullName,
            Guid = user.Guid,
            PrimaryRole = user.PrimaryRole,
            Permissions = user.Permissions
        };
    }
}
