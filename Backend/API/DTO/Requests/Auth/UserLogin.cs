namespace API.DTO.Requests.Auth;

/// <summary>
/// Represents the data required for a user login request.
/// </summary>
/// <remarks>
/// Contains the username and password provided by the user for authentication.
/// </remarks>
public record class UserLogin
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
