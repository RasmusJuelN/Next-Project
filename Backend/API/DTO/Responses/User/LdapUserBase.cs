namespace API.DTO.Responses.User;

/// <summary>
/// Represents the base data transfer object for an LDAP user containing essential user information.
/// </summary>
/// <param name="Id">The unique identifier for the LDAP user.</param>
/// <param name="FullName">The complete display name of the user.</param>
/// <param name="UserName">The username used for authentication and identification in the LDAP system.</param>
public record class LdapUserBase
{
    public required Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string UserName { get; set; }
}
