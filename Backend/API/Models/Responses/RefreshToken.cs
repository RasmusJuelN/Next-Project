namespace API.Models.Responses;

public record class RefreshToken
{
    public required string Token { get; set; }
    public required string TokenType { get; set; }
}
