
namespace Database.Models;

/// <summary>
/// Represents a tracked refresh token used for JWT authentication and session management.
/// Stores token lifecycle information including validity periods and revocation status.
/// </summary>
/// <remarks>
/// This model tracks refresh tokens issued to users for maintaining authenticated sessions.
/// Tokens can be revoked for security purposes and have defined validity periods.
/// The Token field is indexed for efficient lookup during authentication validation.
/// Used to implement secure token rotation and session management policies.
/// </remarks>
[Table("TrackedRefreshToken")]
[Index(nameof(Token))]
public class TrackedRefreshTokenModel
{
    /// <summary>
    /// Gets or sets the unique identifier for this token record.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the GUID of the user this token belongs to.
    /// </summary>
    /// <remarks>
    /// Links the token to a specific user for token management and cleanup operations.
    /// Used to implement per-user token limits and revocation policies.
    /// </remarks>
    [Required]    
    public required Guid UserGuid { get; set; }
    
    /// <summary>
    /// Gets or sets the hashed token value used for authentication validation.
    /// </summary>
    /// <remarks>
    /// Contains the cryptographically hashed version of the refresh token for security.
    /// This field is indexed for efficient token lookup during authentication processes.
    /// The raw token value is never stored in the database for security purposes.
    /// </remarks>
    [Required]
    public required byte[] Token { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when this token becomes valid for use.
    /// </summary>
    /// <remarks>
    /// Default value is configured in Fluent API to use the current timestamp at creation.
    /// Enables implementation of token activation delays or scheduled validity periods.
    /// </remarks>
    [Required]
    public DateTime ValidFrom { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this token expires and becomes invalid.
    /// </summary>
    /// <remarks>
    /// Defines the expiration time for the token. After this time, the token
    /// should not be accepted for authentication even if not explicitly revoked.
    /// Used to implement automatic token cleanup and forced re-authentication.
    /// </remarks>
    [Required]
    public required DateTime ValidUntil { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether this token has been explicitly revoked.
    /// </summary>
    /// <remarks>
    /// When true, the token is immediately invalid regardless of its validity period.
    /// Used for implementing logout, security breach response, and token rotation policies.
    /// Revoked tokens should be rejected during authentication validation.
    /// </remarks>
    [Required]
    public bool IsRevoked { get; set; }
}
