using URLShortener.Services;
using Microsoft.EntityFrameworkCore;
using URLShortener.Data;
using Xunit;

namespace test.UnitTests.Services
{
    public class UrlServiceTests
    {
        private readonly UrlService _urlService;

        private readonly AppDbContext _context;

        public UrlServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "UrlShortenerTestDb")
                .Options;
            _context = new AppDbContext(options);
            _urlService = new UrlService(_context);
        }

        [Fact]
        public async Task CreateShortUrl_ShouldReturnShortCode()
        {
            var originalUrl = "https://www.example.com";
            var shortCode = await _urlService.createShortUrl(originalUrl);
            Assert.NotNull(shortCode);
            Assert.Equal(6, shortCode.Length);
        }

        [Fact]
        public async Task CreateShortUrl_ShouldThrowArgumentException_ForInvalidUrl()
        {
            var invalidUrl = "invalid-url";
            await Assert.ThrowsAsync<ArgumentException>(() => _urlService.createShortUrl(invalidUrl));

        }

        [Fact]
        public async Task CreateShortUrl_ShouldThrowArgumentException_ForEmptyUrl()
        {
            var emptyUrl = "";
            await Assert.ThrowsAsync<ArgumentException>(() => _urlService.createShortUrl(emptyUrl));
        }

        [Fact]
        public async Task CreateShortUrl_ShouldThrowArgumentException_ForNullUrl()
        {
            string nullUrl = null;
            await Assert.ThrowsAsync<ArgumentException>(() => _urlService.createShortUrl(nullUrl));
        }

        [Fact]
        public async Task GetOriginalUrl_ShouldReturnOriginalUrl()
        {
            var originalUrl = "https://www.example.com";
            var shortCode = await _urlService.createShortUrl(originalUrl);
            var retrievedUrl = await _urlService.getOriginalUrl(shortCode);
            Assert.Equal(originalUrl, retrievedUrl);
        }

        [Fact]
        public async Task GetOriginalUrl_ShouldIncrementClickCount()
        {
            var originalUrl = "https://www.example.com";
            var shortCode = await _urlService.createShortUrl(originalUrl);
            var urlMapping = await _context.UrlMappings.FirstOrDefaultAsync(m => m.ShortCode == shortCode);
            var initialClickCount = urlMapping.ClickCount;
            await _urlService.getOriginalUrl(shortCode);
            var updatedClickCount = urlMapping.ClickCount;
            Assert.Equal(initialClickCount + 1, updatedClickCount);
        }

        [Fact]
        public async Task GetOriginalUrl_ShouldThrowArgumentException_ForNonExistentShortCode()
        {
            var nonExistentShortCode = "abcdef";
            await Assert.ThrowsAsync<ArgumentException>(() => _urlService.getOriginalUrl(nonExistentShortCode));

        }
    }
}