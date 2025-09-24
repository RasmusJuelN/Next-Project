using Database;
using Database.Models;
using Database.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTests.ReposTest
{
    public class TrackedRefreshTokenRepositoryTest
    {
        private readonly DbContextOptions<Context> _options;
        public TrackedRefreshTokenRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<Context>()
              .UseInMemoryDatabase(Guid.NewGuid().ToString())
              .Options;
        }
        private TrackedRefreshTokenRepository CreateRepo(Context context)
           => new TrackedRefreshTokenRepository(context, NullLoggerFactory.Instance);

        [Fact]
        public async Task RevokeToken_ByModel_ShouldSetIsRevokedTrue()
        {
            // Arrange
            var context = new Context(_options);
            var repo = CreateRepo(context);

            var token = new TrackedRefreshTokenModel
            {
                UserGuid = Guid.NewGuid(),
                Token = Guid.NewGuid().ToByteArray(),
                ValidFrom = DateTime.UtcNow.AddMinutes(-10),
                ValidUntil = DateTime.UtcNow.AddMinutes(50),
                IsRevoked = false
            };
            context.Add(token);
            await context.SaveChangesAsync();

            // Act
            repo.RevokeToken(token);

            // Assert
            Assert.True(token.IsRevoked);
        }

        [Fact]
        public async Task RevokeToken_ByHashedToken_ShouldSetIsRevokedTrue()
        {
            // Arrange
            var context = new Context(_options);
            var repo = CreateRepo(context);

            var hashedToken = Guid.NewGuid().ToByteArray();
            var token = new TrackedRefreshTokenModel
            {
                UserGuid = Guid.NewGuid(),
                Token = hashedToken,
                ValidFrom = DateTime.UtcNow.AddMinutes(-10),
                ValidUntil = DateTime.UtcNow.AddMinutes(50),
                IsRevoked = false
            };
            context.Add(token);
            await context.SaveChangesAsync();

            // Act
            await repo.RevokeToken(hashedToken);

            // Assert
            Assert.True(token.IsRevoked);
        }

        [Fact]
        public async Task IsTokenRevoked_ShouldReturnTrueIfRevoked()
        {
            // Arrange
            var context = new Context(_options);
            var repo = CreateRepo(context);

            var tokenBytes = Guid.NewGuid().ToByteArray();
            var token = new TrackedRefreshTokenModel
            {
                UserGuid = Guid.NewGuid(),
                Token = tokenBytes,
                ValidFrom = DateTime.UtcNow.AddMinutes(-10),
                ValidUntil = DateTime.UtcNow.AddMinutes(50),
                IsRevoked = true
            };
            context.Add(token);
            await context.SaveChangesAsync();

            // Act
            bool revoked = await repo.IsTokenRevoked(tokenBytes);

            // Assert
            Assert.True(revoked);
        }

        [Fact]
        public async Task RevokeOldTokensUntilThereAreNValid_ShouldRevokeOldTokens()
        {
            // Arrange
            var context = new Context(_options);
            var repo = CreateRepo(context);

            var userGuid = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var tokens = new List<TrackedRefreshTokenModel>();
            for (int i = 0; i < 5; i++)
            {
                tokens.Add(new TrackedRefreshTokenModel
                {
                    UserGuid = userGuid,
                    Token = Guid.NewGuid().ToByteArray(),
                    ValidFrom = now.AddMinutes(i),
                    ValidUntil = now.AddMinutes(i + 10),
                    IsRevoked = false
                });
            }

            context.AddRange(tokens);
            await context.SaveChangesAsync();

            // Act: keep only 2 valid tokens
            await repo.RevokeOldTokensUntilThereAreNValid(userGuid, 2);
            await context.SaveChangesAsync();

            // Assert: only 2 tokens remain in the DB
            var remainingTokens = await context.Set<TrackedRefreshTokenModel>()
                .Where(t => t.UserGuid == userGuid)
                .ToListAsync();

            Assert.Equal(2, remainingTokens.Count);
            Assert.All(remainingTokens, t => Assert.False(t.IsRevoked)); 
        }
    }
}
