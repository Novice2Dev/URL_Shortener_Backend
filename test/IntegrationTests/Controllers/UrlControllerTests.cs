using System.Linq;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using URLShortener.DTO;
using Xunit;

namespace test.IntegrationTests.Controllers
{
    public class UrlControllerTests : IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        public UrlControllerTests()
        {
            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove any existing DbContext registrations
                    var descriptors = services
                        .Where(d => d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true ||
                                    d.ServiceType == typeof(DbContextOptions<URLShortener.Data.AppDbContext>) ||
                                    d.ServiceType == typeof(URLShortener.Data.AppDbContext))
                        .ToList();
                    foreach (var d in descriptors)
                        services.Remove(d);

                    // Add a single shared in-memory database for tests
                    services.AddDbContext<URLShortener.Data.AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            });
        }

        public async Task InitializeAsync()
        {
            _client = _factory.CreateClient();

            // Ensure DB is created
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<URLShortener.Data.AppDbContext>();
            await db.Database.EnsureCreatedAsync();
        }

        public async Task DisposeAsync()
        {
            _client?.Dispose();
            _factory?.Dispose();
            await Task.CompletedTask;
        }

        [Fact]
        public async Task CreateShortUrl_ValidUrl_ReturnsShortUrl()
        {
            var request = new CreateShortUrlRequestDto { OriginalUrl = "https://www.google.com" };

            var response = await _client.PostAsJsonAsync("/api/shorten", request);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ShortUrlResponseDto>();

            Assert.NotNull(result);
            Assert.NotNull(result.ShortUrl);
            Assert.Contains("https://localhost", result.ShortUrl);
        }

        [Fact]
        public async Task CreateShortUrl_InvalidUrl_ReturnsBadRequest()
        {
            var request = new CreateShortUrlRequestDto { OriginalUrl = "invalid-url" };
            var response = await _client.PostAsJsonAsync("/api/shorten", request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RedirectToOriginalUrl_InvalidShortCode_ReturnsNotFound()
        {
            var invalidShortCode = "invalid123";
            var response = await _client.GetAsync($"/{invalidShortCode}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}