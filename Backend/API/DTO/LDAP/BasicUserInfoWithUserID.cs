namespace API.DTO.LDAP;

/// <summary>
/// Represents basic user information extended with the users ID.
/// Inherits from <see cref="BasicUserInfo"/>.
/// </summary>
public record class BasicUserInfoWithUserID : BasicUserInfo
{
    /// <summary>
    /// Gets or sets the user ID of the user.
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}
