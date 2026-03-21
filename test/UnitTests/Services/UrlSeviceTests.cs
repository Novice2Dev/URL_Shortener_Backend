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
    }
}