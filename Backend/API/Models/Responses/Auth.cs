namespace API.Models.Responses;

public record class Auth
{
    public required AuthenticationToken AuthToken { get; set; }
    public required RefreshToken RefreshToken { get; set; }
}
