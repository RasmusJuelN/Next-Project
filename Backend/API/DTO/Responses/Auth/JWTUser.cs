namespace API.DTO.Responses.Auth;

public record class JWTUser
{
    public required Guid Guid { get; set; }
    public required string Username { get; set; }
    public required string Name { get; set; }
    public required string Role { get; set; }
    public required int Permissions { get; set; }
}
