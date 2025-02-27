using System.Threading.Tasks;
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

    public async Task RevokeOldTokensUntilThereAreNValid(Guid id, int n)
    {
        List<TrackedRefreshTokenModel> trackedRefreshTokens = await GetAsync(q => q.UserGuid == id, query => query.OrderBy(t => t.ValidFrom));

        if (trackedRefreshTokens.Count <= n)
        {
            return;
        }

        List<TrackedRefreshTokenModel> tokensToRevoke = [.. trackedRefreshTokens.Take(trackedRefreshTokens.Count - n)];

        DeleteRange(tokensToRevoke);
        return;
    }

    public async Task<bool> IsTokenRevoked(byte[] token)
    {
        TrackedRefreshTokenModel? trackedRefreshToken = await GetSingleAsync(t => t.Token == token);

        if (trackedRefreshToken is null) return false;

        return trackedRefreshToken.IsRevoked;
    }
}
