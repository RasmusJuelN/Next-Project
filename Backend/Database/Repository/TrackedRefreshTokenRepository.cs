using Database.Interfaces;
using Database.Models;
using Microsoft.Extensions.Logging;

namespace Database.Repository;

public class TrackedRefreshTokenRepository(Context context, ILoggerFactory loggerFactory) : SQLGenericRepository<TrackedRefreshTokenModel>(context, loggerFactory), ITrackedRefreshTokenRepository
{
    private readonly Context _context = context;

    public void RevokeToken(TrackedRefreshTokenModel trackedRefreshToken)
    {
        trackedRefreshToken.IsRevoked = true;
        return;
    }

    public async Task RevokeToken(byte[] hashedRefreshToken)
    {
        TrackedRefreshTokenModel trackedRefreshToken = await GetSingleAsync(t => t.Token == hashedRefreshToken) ?? throw new Exception("Token not found");

        trackedRefreshToken.IsRevoked = true;
        return;
    }

    public async Task RevokeOldTokensUntilThereAreNValid(Guid id, int n)
    {
        List<TrackedRefreshTokenModel> trackedRefreshTokens = await GetAsync(q => q.UserGuid == id, query => query.OrderBy(t => t.ValidFrom));

        if (trackedRefreshTokens.Count <= n)
        {
            return;
        }

        List<TrackedRefreshTokenModel> tokensToRevoke = [.. trackedRefreshTokens.Take(trackedRefreshTokens.Count - n)];

        foreach (TrackedRefreshTokenModel tokenToRevoke in tokensToRevoke)
        {
            if (tokenToRevoke.ValidTo > DateTime.UtcNow)
            {
                Delete(tokenToRevoke);
            }
            else
            {
                tokenToRevoke.IsRevoked = true;
            }
        }
        return;
    }

    public async Task<bool> IsTokenRevoked(byte[] token)
    {
        TrackedRefreshTokenModel? trackedRefreshToken = await GetSingleAsync(t => t.Token == token);

        if (trackedRefreshToken is null) return false;

        return trackedRefreshToken.IsRevoked;
    }
}
