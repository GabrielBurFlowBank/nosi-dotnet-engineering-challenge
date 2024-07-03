using Microsoft.Extensions.Caching.Memory;
using NOS.Engineering.Challenge.Cache;
using Moq;

namespace NOS.Engineering.Challenge.Tests.Cache;

public class InMemoryCacheServiceTest
{
    private readonly Mock<IMemoryCache> _memoryCacheMock;
    private readonly InMemoryCacheService _inMemoryCacheService;

    public InMemoryCacheServiceTest()
    {
        _memoryCacheMock = new Mock<IMemoryCache>();
        _inMemoryCacheService = new InMemoryCacheService(_memoryCacheMock.Object);
    }

    [Fact]
    public async Task GetOrSetAsync_ShouldSetCacheValue_WhenValueIsNotInCache()
    {
        // Arrange
        string cacheKey = "test_key";
        string expectedValue = "new_value";
        object cacheValue = null;
        _memoryCacheMock.Setup(x => x.TryGetValue(cacheKey, out cacheValue)).Returns(false);
        _memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

        // Act
        var result = await _inMemoryCacheService.GetOrSetAsync(cacheKey, () => Task.FromResult(expectedValue));

        // Assert
        Assert.Equal(expectedValue, result);
        _memoryCacheMock.Verify(x => x.CreateEntry(It.Is<object>(k => (string)k == cacheKey)), Times.Once);
    }

    [Fact]
    public async Task SetAsync_ShouldSetCacheValue()
    {
        // Arrange
        var cacheKey = Guid.NewGuid();
        string value = "test_value";

        _memoryCacheMock.Setup(x => x.CreateEntry(cacheKey)).Returns(Mock.Of<ICacheEntry>());

        // Act
        await _inMemoryCacheService.SetAsync(cacheKey, value);

        // Assert
        _memoryCacheMock.Verify(x => x.CreateEntry(It.Is<object>(k => (Guid)k == cacheKey)), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveCacheValue()
    {
        // Arrange
        var cacheKey = Guid.NewGuid();

        // Act
        await _inMemoryCacheService.RemoveAsync(cacheKey);

        // Assert
        _memoryCacheMock.Verify(x => x.Remove(It.Is<object>(k => (Guid)k == cacheKey)), Times.Once);
    }

}
