using System;
using System.Threading.Tasks;
using FluentAssertions;
using HRMS.Infrastructure.Services;
using Xunit;

namespace HRMS.UnitTests.Services
{
    public class InMemoryCacheServiceTests
    {
        private readonly InMemoryCacheService _cacheService;

        public InMemoryCacheServiceTests()
        {
            _cacheService = new InMemoryCacheService();
        }

        [Fact]
        public async Task SetAsync_StoresValue_Succeeds()
        {
            // Arrange
            var key = "test_key";
            var value = "test_value";

            // Act
            await _cacheService.SetAsync(key, value, TimeSpan.FromMinutes(5));

            // Assert - verify we can retrieve it
            var retrieved = await _cacheService.GetAsync<string>(key);
            retrieved.Should().Be(value);
        }

        [Fact]
        public async Task GetAsync_RetrievesStoredValue()
        {
            // Arrange
            var key = "cache_key";
            var value = "cached_data";
            await _cacheService.SetAsync(key, value, TimeSpan.FromMinutes(5));

            // Act
            var result = await _cacheService.GetAsync<string>(key);

            // Assert
            result.Should().Be(value);
        }

        [Fact]
        public async Task GetAsync_KeyNotFound_ReturnsNull()
        {
            // Arrange
            var key = "nonexistent_key";

            // Act
            var result = await _cacheService.GetAsync<string>(key);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ExistsAsync_KeyExists_ReturnsTrue()
        {
            // Arrange
            var key = "existing_key";
            await _cacheService.SetAsync(key, "value", TimeSpan.FromMinutes(5));

            // Act
            var exists = await _cacheService.ExistsAsync(key);

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_KeyNotExists_ReturnsFalse()
        {
            // Arrange
            var key = "missing_key";

            // Act
            var exists = await _cacheService.ExistsAsync(key);

            // Assert
            exists.Should().BeFalse();
        }

        [Fact]
        public async Task RemoveAsync_DeletesKey()
        {
            // Arrange
            var key = "key_to_delete";
            await _cacheService.SetAsync(key, "value", TimeSpan.FromMinutes(5));

            // Act
            await _cacheService.RemoveAsync(key);
            var stillExists = await _cacheService.ExistsAsync(key);

            // Assert
            stillExists.Should().BeFalse();
        }

        [Fact]
        public async Task RemoveByPatternAsync_RemovesMatchingKeys()
        {
            // Arrange
            await _cacheService.SetAsync("user:1:profile", "data1", TimeSpan.FromMinutes(5));
            await _cacheService.SetAsync("user:2:profile", "data2", TimeSpan.FromMinutes(5));
            await _cacheService.SetAsync("user:1:settings", "data3", TimeSpan.FromMinutes(5));
            await _cacheService.SetAsync("admin:1:data", "data4", TimeSpan.FromMinutes(5));

            // Act - RemoveByPatternAsync uses Contains() match, not wildcard
            await _cacheService.RemoveByPatternAsync("user:");

            // Assert - all keys containing "user:" should be removed
            (await _cacheService.ExistsAsync("user:1:profile")).Should().BeFalse();
            (await _cacheService.ExistsAsync("user:1:settings")).Should().BeFalse();
            (await _cacheService.ExistsAsync("user:2:profile")).Should().BeFalse();
            (await _cacheService.ExistsAsync("admin:1:data")).Should().BeTrue(); // Not matching pattern
        }

        [Fact]
        public async Task SetAsync_WithExpiration_ValueExpires()
        {
            // Arrange
            var key = "expiring_key";
            var value = "will_expire";
            await _cacheService.SetAsync(key, value, TimeSpan.FromMilliseconds(100));

            // Act
            var immediate = await _cacheService.GetAsync<string>(key);
            await Task.Delay(150); // Wait for expiration
            var afterExpiration = await _cacheService.GetAsync<string>(key);

            // Assert
            immediate.Should().Be(value);
            afterExpiration.Should().BeNull();
        }
    }
}
