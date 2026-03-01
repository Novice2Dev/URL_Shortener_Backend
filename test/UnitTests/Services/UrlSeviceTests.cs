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
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(options);
            _urlService = new UrlService(_context);
        }

        [Fact]
        public void GenerateShortCode_ShouldReturn6CharacterString()
        {
            var shortCode = _urlService.generateShortCode();

            Assert.NotNull(shortCode);
            Assert.Equal(6, shortCode.Length);
        }

        [Fact]
        public void GenerateShortCode_ShouldContainOnlyAlphanumericCharacters()
        {
            var code = _urlService.generateShortCode();

            Assert.Matches("^[a-zA-Z0-9]+$", code);
        }
    }
}