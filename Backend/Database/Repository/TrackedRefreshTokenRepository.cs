
namespace Database.Repository;

/// <summary>
/// Implements repository operations for refresh token management.
/// Provides functionality for JWT refresh token lifecycle management including revocation, validation, and cleanup operations for secure session management.
/// </summary>
/// <remarks>
/// This repository manages the security-critical aspects of refresh token handling, including token revocation
/// for security purposes, cleanup policies to prevent token accumulation, and validation to ensure only
/// legitimate tokens are accepted. All operations are designed with security best practices in mind.
/// </remarks>
/// <param name="context">The database context for data access operations.</param>
/// <param name="loggerFactory">Factory for creating loggers for diagnostic and monitoring purposes.</param>
public class TrackedRefreshTokenRepository(Context context, ILoggerFactory loggerFactory) : ITrackedRefreshTokenRepository
{
    private readonly Context _context = context;
    private readonly GenericRepository<TrackedRefreshTokenModel> _genericRepository = new(context, loggerFactory);

    /// <summary>
    /// Revokes a specific refresh token by marking it as invalid.
    /// </summary>
    /// <param name="trackedRefreshToken">The TrackedRefreshTokenModel representing the token to revoke.</param>
    /// <remarks>
    /// This method immediately marks the token as revoked, preventing its future use for authentication.
    /// The token model must be tracked by the database context for this operation to succeed.
    /// Used for immediate token invalidation in security scenarios.
    /// </remarks>
    public void RevokeToken(TrackedRefreshTokenModel trackedRefreshToken)
    {
        trackedRefreshToken.IsRevoked = true;
        return;
    }

    /// <summary>
    /// Revokes a refresh token by its hashed value.
    /// </summary>
    /// <param name="hashedRefreshToken">The hashed bytes of the refresh token to revoke.</param>
    /// <exception cref="Exception">Thrown when the token is not found in the database.</exception>
    /// <remarks>
    /// This method looks up the token by its hash and marks it as revoked if found.
    /// Used when only the token hash is available without the full model context.
    /// Essential for logout operations and security breach responses.
    /// </remarks>
    public async Task RevokeToken(byte[] hashedRefreshToken)
    {
        TrackedRefreshTokenModel trackedRefreshToken = await _genericRepository.GetSingleAsync(t => t.Token == hashedRefreshToken) ?? throw new Exception("Token not found");

        trackedRefreshToken.IsRevoked = true;
        return;
    }

    /// <summary>
    /// Revokes old refresh tokens for a user, keeping only the specified number of most recent valid tokens.
    /// </summary>
    /// <param name="id">The user GUID whose tokens should be cleaned up.</param>
    /// <param name="n">The number of most recent tokens to keep valid.</param>
    /// <remarks>
    /// This method helps prevent token accumulation by automatically revoking or deleting older tokens.
    /// Tokens are ordered by ValidFrom date, with the oldest tokens being processed first.
    /// Expired tokens are deleted while valid tokens are revoked to maintain audit trails.
    /// Useful for implementing token rotation policies and limiting concurrent sessions per user.
    /// </remarks>
    public async Task RevokeOldTokensUntilThereAreNValid(Guid id, int n)
    {
        List<TrackedRefreshTokenModel> trackedRefreshTokens = await _genericRepository.GetAsync(q => q.UserGuid == id, query => query.OrderBy(t => t.ValidFrom));

        if (trackedRefreshTokens.Count <= n)
        {
            return;
        }

        List<TrackedRefreshTokenModel> tokensToRevoke = [.. trackedRefreshTokens.Take(trackedRefreshTokens.Count - n)];

        foreach (TrackedRefreshTokenModel tokenToRevoke in tokensToRevoke)
        {
            if (tokenToRevoke.ValidUntil > DateTime.UtcNow)
            {
                _genericRepository.Delete(tokenToRevoke);
            }
            else
            {
                tokenToRevoke.IsRevoked = true;
            }
        }
        return;
    }

    /// <summary>
    /// Checks if a specific refresh token has been revoked.
    /// </summary>
    /// <param name="token">The hashed bytes of the token to check.</param>
    /// <returns>True if the token is revoked, false if the token is valid or not found.</returns>
    /// <remarks>
    /// This method is used during authentication to validate token status before processing refresh requests.
    /// Returns false for tokens that don't exist in the database, allowing the authentication flow
    /// to handle missing tokens appropriately. Only returns true for explicitly revoked tokens.
    /// </remarks>
    public async Task<bool> IsTokenRevoked(byte[] token)
    {
        TrackedRefreshTokenModel? trackedRefreshToken = await _genericRepository.GetSingleAsync(t => t.Token == token);

        if (trackedRefreshToken is null) return false;

        return trackedRefreshToken.IsRevoked;
    }
}
