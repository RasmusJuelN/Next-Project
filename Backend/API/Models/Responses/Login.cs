namespace API.Models.Responses;

public record class Login
{
    public required AuthenticationToken AuthToken { get; set; }
    public required RefreshToken RefreshToken { get; set; }
}
