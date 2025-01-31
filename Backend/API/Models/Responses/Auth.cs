namespace API.Models.Responses;

public record class AuthenticationResponse
{
    public required string AuthToken { get; set; }
    public required string RefreshToken { get; set; }
}
