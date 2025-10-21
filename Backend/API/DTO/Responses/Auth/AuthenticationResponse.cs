namespace API.DTO.Responses.Auth;

/// <summary>
/// Represents the response returned after successful user authentication.
/// </summary>
/// <param name="AuthToken">The JWT access token used for authenticating API requests.</param>
/// <param name="RefreshToken">The refresh token used to obtain new access tokens when they expire.</param>
public record class AuthenticationResponse
{
    public required string AuthToken { get; set; }
    public required string RefreshToken { get; set; }
}
