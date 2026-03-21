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
        private HttpClient? _client;

        public UrlControllerTests()
        {
            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove all DbContext and related EF Core services
                    var descriptorsToRemove = services.Where(
                        d => d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true ||
                             d.ServiceType == typeof(DbContextOptions<URLShortener.Data.AppDbContext>) ||
                             d.ServiceType == typeof(URLShortener.Data.AppDbContext)).ToList();

                    foreach (var descriptor in descriptorsToRemove)
                    {
                        services.Remove(descriptor);
                    }

                    // Add in-memory database for testing
                    services.AddDbContext<URLShortener.Data.AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString());
                    });
                });
            });
        }

        public async Task InitializeAsync()
        {
            _client = _factory.CreateClient();

            // Ensure database schema is created
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<URLShortener.Data.AppDbContext>();
                await dbContext.Database.EnsureCreatedAsync();
            }
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
            // Arrange
            var request = new CreateShortUrlRequestDto
            {
                OriginalUrl = "https://www.google.com"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/shorten", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ShortUrlResponseDto>();
            Assert.NotNull(result);
            Assert.NotNull(result.ShortUrl);
            Assert.Contains("https://localhost", result.ShortUrl);
        }

        [Fact]
        public async Task CreateShortUrl_InvalidUrl_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateShortUrlRequestDto
            {
                OriginalUrl = "invalid-url"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/shorten", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateShortUrl_EmptyUrl_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateShortUrlRequestDto
            {
                OriginalUrl = ""
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/shorten", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}