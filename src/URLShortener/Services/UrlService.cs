using URLShortener.Data;

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
            return "";
        }

        public async Task<string> getOriginalUrl(string code)
        {
            return "";
        }

        public string generateShortCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            do
            {
                var shortCode = new string(Enumerable.Repeat(chars, 6)
                  .Select(s => s[new Random().Next(s.Length)]).ToArray());
                if (!_context.UrlMappings.Any(m => m.ShortCode == shortCode))
                    return shortCode;
            } while (true);
        }
    }
}