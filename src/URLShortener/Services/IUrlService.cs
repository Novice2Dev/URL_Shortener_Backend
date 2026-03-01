namespace URLShortener.Services
{
    public interface IUrlService
    {
        Task<string> createShortUrl(String originalUrl);
        Task<string> getOriginalUrl(String code);
    }
}


