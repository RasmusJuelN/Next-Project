namespace API.DTO.Responses.Auth;

public record class AuthenticationResponse
{
    public required string AuthToken { get; set; }
    public required string RefreshToken { get; set; }
}
