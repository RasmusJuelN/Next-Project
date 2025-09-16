namespace Database.DTO.User;

/// <summary>
/// Represents the base user information containing essential user properties.
/// </summary>
/// <remarks>
/// This record class serves as a foundation for user-related data transfer objects,
/// providing common properties that identify and describe a user.
/// </remarks>
public record class UserBase
{
    public required string UserName { get; set; }
    public required string FullName { get; set; }
}
