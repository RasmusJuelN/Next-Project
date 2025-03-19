namespace API.DTO.Responses.User;

public record class UserQueryPaginationResult
{
    public required List<LdapUserBase> UserBases { get; set; }
    public required string SessionId { get; set; }
    public required bool HasMore { get; set; }
}
