namespace API.DTO.Responses.User;

/// <summary>
/// Represents the result of a paginated user query operation.
/// </summary>
/// <remarks>
/// This record contains the retrieved user data along with pagination metadata
/// to support chunked loading of user information from LDAP sources.
/// </remarks>
public record class UserQueryPaginationResult
{
    public required List<LdapUserBase> UserBases { get; set; }
    public required string SessionId { get; set; }
    public required bool HasMore { get; set; }
}
