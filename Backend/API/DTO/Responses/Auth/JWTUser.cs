namespace API.DTO.Responses.Auth;

/// <summary>
/// Represents a JWT (JSON Web Token) user containing essential user information for authentication and authorization.
/// </summary>
/// <param name="Guid">The unique identifier for the user.</param>
/// <param name="Username">The user's login username.</param>
/// <param name="Name">The user's display name or full name.</param>
/// <param name="Role">The user's role in the system.</param>
/// <param name="Permissions">The user's permission level represented as an integer value.</param>
public record class JWTUser
{
    public required Guid Guid { get; set; }
    public required string Username { get; set; }
    public required string Name { get; set; }
    public required string Role { get; set; }
    public required int Permissions { get; set; }
}
