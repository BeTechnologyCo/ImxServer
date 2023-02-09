using ImxServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Nethereum.Signer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ImxServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly ILogger<PlayerController> _logger;
        private readonly GameContext _dbContext;

        public PlayerController(ILogger<PlayerController> logger, IConfiguration config, IMemoryCache cache, GameContext dbContext)
        {
            _logger = logger;
            _config = config;
            _cache = cache;
            _dbContext = dbContext;
        }


        [HttpGet("GetPlayer")]
        public Player GetPlayer()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claimAccount = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Email);

            var player = _dbContext.Players.Where(a => a.Account == claimAccount.Value.ToLowerInvariant()).FirstOrDefault();

            return player;
        }

        [HttpPost("CreatePlayer")]
        public async Task<IActionResult> CreatePlayer([FromBody] PlayerName playerName)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claimAccount = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Email);

            var player = _dbContext.Players.Where(a => a.Account == claimAccount.Value.ToLowerInvariant()).FirstOrDefault();

            if (player != null)
            {

                return Conflict("Player already exist");
            }
            else
            {
                var playerSameName = _dbContext.Players.Where(a => a.Name.ToLowerInvariant() == playerName.Name.ToLowerInvariant()).FirstOrDefault();

                var playerEntity = new Player()
                {
                    // register Account as lower
                    Account = claimAccount.Value.ToLowerInvariant(),
                    Name = playerName.Name
                };

                _dbContext.Players.Add(playerEntity);
                _dbContext.SaveChanges();


                return Ok(playerEntity);
            }
        }
    }
}
