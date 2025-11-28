
namespace Database.Extensions;

/// <summary>
/// Provides extension methods for mapping user base database models to DTOs.
/// This mapper handles conversions from Entity Framework base user models to response DTOs for API endpoints.
/// </summary>
public static class UserBaseModelMapper
{
    /// <summary>
    /// Converts a UserBaseModel to a UserBase DTO containing essential user information.
    /// </summary>
    /// <param name="user">The UserBaseModel from the database.</param>
    /// <returns>A UserBase DTO with basic user identification information.</returns>
    /// <remarks>
    /// This method creates a lightweight DTO containing only essential user identification details.
    /// Useful for scenarios where minimal user information is required, such as user lists or
    /// reference displays where security-sensitive details are not needed.
    /// </remarks>
    public static UserBase ToBaseDto(this UserBaseModel user)
    {
        return new()
        {
            UserName = user.UserName,
            FullName = user.FullName
        };
    }

    /// <summary>
    /// Converts a UserBaseModel to a FullUser DTO containing complete user information.
    /// </summary>
    /// <param name="user">The UserBaseModel from the database.</param>
    /// <returns>A FullUser DTO with comprehensive user details including LDAP information, roles, and permissions.</returns>
    /// <remarks>
    /// This method creates a comprehensive DTO containing all user information including security-sensitive
    /// details like GUID, roles, and permissions. Should only be used in contexts where the requesting
    /// user has appropriate authorization to view this detailed information.
    /// </remarks>
    public static FullUser ToDto(this UserBaseModel user)
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
