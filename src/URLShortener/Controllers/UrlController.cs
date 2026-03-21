using Microsoft.AspNetCore.Mvc;
using URLShortener.Services;
using URLShortener.DTO;

namespace URLShortener.Controllers
{
    [ApiController]
    [Route("api/shorten")]
    public class UrlController : ControllerBase
    {
        private readonly IUrlService _urlService;

        public UrlController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        [HttpPost]
        public async Task<ActionResult<ShortUrlResponseDto>> Create(
        CreateShortUrlRequestDto request)
        {
            try
            {
                var entity = await _urlService.createShortUrl(request.OriginalUrl);
                var response = new ShortUrlResponseDto
                {
                    ShortUrl = $"https://localhost:5157/{entity}"
                };
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}