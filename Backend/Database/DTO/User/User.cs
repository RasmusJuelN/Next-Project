using Database.Enums;

namespace Database.DTO.User;

/// <summary>
/// Represents a complete user entity with full details including unique identifier, role, and permissions.
/// This record extends UserBase to provide a comprehensive user representation for internal operations.
/// </summary>
/// <remarks>
/// This record is typically used when full user information is required, such as for authentication,
/// authorization, and administrative operations. It includes the user's unique identifier, primary role,
/// and detailed permissions.
/// </remarks>
public record class FullUser : UserBase
{
    public required Guid Guid { get; set; }
    public required UserRoles PrimaryRole { get; set; }
    public required UserPermissions Permissions { get; set; }
}
