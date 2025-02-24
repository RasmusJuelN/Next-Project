namespace API.DTO.Requests.Auth;

public record class UserLogin
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
