using Infrastructure.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Newtonsoft.Json;

namespace Infrastructure.Tests.Cache;

public class HybridCacheServiceTests
{
    private readonly Mock<IMemoryCache> _memoryCacheMock;
    private readonly Mock<IDistributedCache> _distributedCacheMock;
    private readonly HybridCacheService _cacheService;

    public HybridCacheServiceTests()
    {
        _memoryCacheMock = new Mock<IMemoryCache>();
        _distributedCacheMock = new Mock<IDistributedCache>();
        _cacheService = new HybridCacheService(_memoryCacheMock.Object, _distributedCacheMock.Object);
    }

    [Fact]
    public async Task GetOrSetAsync_ShouldReturnFromMemoryCache_WhenPresent()
    {
        // Arrange
        var key = "test_key";
        var expectedValue = new TestClass { Id = 1, Name = "Test" };
        object? cacheValue = expectedValue;

        _memoryCacheMock.Setup(m => m.TryGetValue(key, out cacheValue)).Returns(true);

        // Act
        var result = await _cacheService
            .GetOrSetAsync<TestClass>(key, async () => await Task.FromResult<TestClass?>(null));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedValue.Id, result!.Id);
        Assert.Equal(expectedValue.Name, result.Name);
    }

    [Fact]
    public async Task GetOrSetAsync_ShouldReturnFromDistributedCache_WhenMemoryCacheIsEmpty()
    {
        // Arrange
        var key = "test_key";
        var expectedValue = new TestClass { Id = 2, Name = "Redis" };
        var serializedValue = JsonConvert.SerializeObject(expectedValue);
        var cachedBytes = System.Text.Encoding.UTF8.GetBytes(serializedValue);
        object? cacheValue = null;

        _memoryCacheMock.Setup(m => m.TryGetValue(key, out cacheValue)).Returns(false);
        _distributedCacheMock.Setup(d => d.GetAsync(key, default)).ReturnsAsync(cachedBytes);

        var memoryCacheStore = new Dictionary<string, object>();
        _memoryCacheMock
            .Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns((string key) =>
            {
                var mockEntry = new Mock<ICacheEntry>();
                mockEntry.SetupAllProperties();
                mockEntry.Setup(e => e.Dispose()).Callback(() =>
                {
                    if (mockEntry.Object.Value != null)
                        memoryCacheStore[key.ToString()] = mockEntry.Object.Value;
                });
                return mockEntry.Object;
            });

        // Act
        var result = await _cacheService
            .GetOrSetAsync<TestClass>(key, async () => await Task.FromResult<TestClass?>(null));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedValue.Id, result!.Id);
        Assert.Equal(expectedValue.Name, result.Name);
        Assert.True(memoryCacheStore.ContainsKey(key));
    }

    [Fact]
    public async Task GetOrSetAsync_ShouldFetchFromDb_WhenCacheIsEmpty()
    {
        // Arrange
        var key = "test_key";
        var expectedValue = new TestClass { Id = 3, Name = "Database" };
        object? cacheValue = null;

        _memoryCacheMock
            .Setup(m => m.TryGetValue(key, out cacheValue))
            .Returns(false);

        _distributedCacheMock
            .Setup(d => d.GetAsync(key, default))
            .ReturnsAsync((byte[]?)null);

        var memoryCacheStore = new Dictionary<string, object>();

        _memoryCacheMock
            .Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns((string cacheKey) =>
            {
                var mockEntry = new Mock<ICacheEntry>();
                mockEntry.SetupAllProperties();
                mockEntry.Setup(e => e.Dispose()).Callback(() =>
                {
                    if(mockEntry.Object.Value  != null)
                        memoryCacheStore[cacheKey.ToString()] = mockEntry.Object.Value;
                });
                return mockEntry.Object;
            });

        _distributedCacheMock
            .Setup(d => d.SetAsync(key, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _cacheService
            .GetOrSetAsync<TestClass>(key, async () => await Task.FromResult<TestClass?>(expectedValue));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedValue.Id, result!.Id);
        Assert.Equal(expectedValue.Name, result.Name);
        Assert.True(memoryCacheStore.ContainsKey(key));
    }


    [Fact]
    public async Task RemoveAsync_ShouldRemoveFromBothCaches()
    {
        // Arrange
        var key = "test_key";

        // Act
        await _cacheService.RemoveAsync(key);

        // Assert
        _memoryCacheMock.Verify(m => m.Remove(key), Times.Once);
        _distributedCacheMock.Verify(d => d.RemoveAsync(key, default), Times.Once);
    }

    private class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}