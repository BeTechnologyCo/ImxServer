using ImxServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ImxServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NftController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly ILogger<NftController> _logger;
        private readonly GameContext _dbContext;

        public NftController(ILogger<NftController> logger, IConfiguration config, IMemoryCache cache, GameContext dbContext)
        {
            _logger = logger;
            _config = config;
            _cache = cache;
            _dbContext = dbContext;
        }

        [HttpGet("{id}")]
        public NftDto Get(int id)
        {
            var uri = BaseUrl(Request);
            var monster = _dbContext.Tokens.Include(x => x.Monster).Where(x => x.TokenId == id).FirstOrDefault();
            if (monster != null)
            {
                return new NftDto()
                {
                    Name = monster.Monster.Name,
                    Description = "Illumon",
                    Id = monster.Monster.MonsterId,
                    Image = $"{uri}api/nft/image/{monster.Monster.MonsterId}"
                };
            }
            return new NftDto()
            {
                Name = "Egg",
                Description = "Unrevelated monster",
                Id = 0,
                Image = $"{uri}api/nft/image/0"
            };
        }


        [HttpGet("Image/{id}")]
        public IActionResult GetImage(int id)
        {
            if (id < 1 || id > 6)
            {
                var imageNF = System.IO.File.OpenRead("Assets/Monsters/0_0.png");
                return File(imageNF, "image/jpeg");
            }
            var image = System.IO.File.OpenRead($"Assets/Monsters/{id}_0.png");
            return File(image, "image/jpeg");
        }

        public static string? BaseUrl(HttpRequest req)
        {
            if (req == null) return null;
            var uriBuilder = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port ?? -1);
            if (uriBuilder.Uri.IsDefaultPort)
            {
                uriBuilder.Port = -1;
            }

            return uriBuilder.Uri.AbsoluteUri;
        }
    }
}
