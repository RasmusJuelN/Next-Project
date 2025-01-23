namespace API.Models.Responses;

public record class Token
{
    public required string AccessToken { get; set; }
    public required string TokenType { get; set; }
}
