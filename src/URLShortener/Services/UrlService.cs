using URLShortener.Data;
using URLShortener.Helpers;
using URLShortener.Models;

namespace URLShortener.Services
{
    public class UrlService : IUrlService
    {
        private readonly AppDbContext _context;

        public UrlService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> createShortUrl(string originalURL)
        {
            if (string.IsNullOrEmpty(originalURL))
            {
                throw new ArgumentException("Original URL cannot be null or empty.");
            }
            if (!Uri.IsWellFormedUriString(originalURL, UriKind.Absolute))
            {
                throw new ArgumentException("Invalid URL format.");
            }
            var shortCode = ShortCodeGenerator.GenerateShortCode();

            var urlMapping = new UrlMapping
            {
                OriginalUrl = originalURL,
                ShortCode = shortCode
            };

            _context.UrlMappings.Add(urlMapping);
            await _context.SaveChangesAsync();

            return shortCode;
        }

        public async Task<string> getOriginalUrl(string code)
        {
            return "";
        }
    }
}