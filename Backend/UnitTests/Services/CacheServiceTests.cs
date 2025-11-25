
namespace UnitTests.Services
{
    public class CacheServiceTests
    {

            private readonly IMemoryCache _memoryCache;
            private readonly ILogger<CacheService> _logger;
            private readonly CacheService _cacheService;

            public CacheServiceTests()
            {
                _memoryCache = new MemoryCache(new MemoryCacheOptions());
                _logger = Mock.Of<ILogger<CacheService>>();
                _cacheService = new CacheService(_memoryCache, _logger);
            }

            [Fact]
            public void SetAndGet_ShouldReturnStoredObject()
            {
                string value = "test-value";
                string sessionId = _cacheService.Set(value);

                var result = _cacheService.Get<string>(sessionId);

                Assert.Equal(value, result);
            }

            [Fact]
            public void Remove_ShouldDeleteObjectFromCache()
            {
                string value = "test-value";
                string sessionId = _cacheService.Set(value);

                _cacheService.Remove(sessionId);

                var result = _cacheService.Get<string>(sessionId);
                Assert.Null(result);
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData(" ")]
            public void Set_WithInvalidSessionId_ShouldThrow(string sessionId)
            {
                Assert.Throws<ArgumentException>(() => _cacheService.Set(sessionId, "value"));
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData(" ")]
            public void Get_WithInvalidSessionId_ShouldThrow(string sessionId)
            {
                Assert.Throws<ArgumentException>(() => _cacheService.Get<string>(sessionId));
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData(" ")]
            public void Remove_WithInvalidSessionId_ShouldThrow(string sessionId)
            {
                Assert.Throws<ArgumentException>(() => _cacheService.Remove(sessionId));
            }
        }

}
