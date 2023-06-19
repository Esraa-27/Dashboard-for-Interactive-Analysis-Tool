using Microsoft.AspNetCore.Http;

namespace MarketApi.Dtos.Shared
{
    public class FileDto
    {
        public IFormFile File { set; get; }
    }
}
