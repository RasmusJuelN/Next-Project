namespace API.DTO.Responses.User;

public record class LdapUserBase
{
    public required Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string UserName { get; set; }
}
