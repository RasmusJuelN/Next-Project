
namespace Database.Interfaces;

/// <summary>
/// Defines the contract for refresh token repository operations.
/// Manages the lifecycle of JWT refresh tokens including revocation, validation, and cleanup operations.
/// </summary>
public interface ITrackedRefreshTokenRepository
{
    /// <summary>
    /// Revokes a specific refresh token by marking it as invalid.
    /// </summary>
    /// <param name="trackedRefreshToken">The TrackedRefreshTokenModel representing the token to revoke.</param>
    /// <remarks>
    /// This method immediately marks the token as revoked, preventing its future use for authentication.
    /// The token model must be tracked by the database context for this operation to succeed.
    /// </remarks>
    void RevokeToken(TrackedRefreshTokenModel trackedRefreshToken);

    /// <summary>
    /// Revokes a refresh token by its hashed value.
    /// </summary>
    /// <param name="hashedRefreshToken">The hashed bytes of the refresh token to revoke.</param>
    /// <remarks>
    /// This method looks up the token by its hash and marks it as revoked if found.
    /// Used when only the token hash is available without the full model context.
    /// </remarks>
    Task RevokeToken(byte[] hashedRefreshToken);

    /// <summary>
    /// Revokes old refresh tokens for a user, keeping only the specified number of most recent valid tokens.
    /// </summary>
    /// <param name="id">The user ID whose tokens should be cleaned up.</param>
    /// <param name="n">The number of most recent tokens to keep valid.</param>
    /// <remarks>
    /// This method helps prevent token accumulation by automatically revoking older tokens.
    /// Useful for implementing token rotation policies and limiting concurrent sessions per user.
    /// The most recently created tokens are preserved while older ones are revoked.
    /// </remarks>
    Task RevokeOldTokensUntilThereAreNValid(Guid id, int n);

    /// <summary>
    /// Checks if a specific refresh token has been revoked.
    /// </summary>
    /// <param name="token">The hashed bytes of the token to check.</param>
    /// <returns>True if the token is revoked or not found, false if the token is still valid.</returns>
    /// <remarks>
    /// This method is used during authentication to validate token status before processing refresh requests.
    /// Returns true for both explicitly revoked tokens and tokens that don't exist in the database.
    /// </remarks>
    Task<bool> IsTokenRevoked(byte[] token);
}
