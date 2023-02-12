using ImxServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class RegisterController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RegisterController> _logger;
        private readonly GameContext _dbContext;

        public RegisterController(ILogger<RegisterController> logger, IConfiguration config, IMemoryCache cache, GameContext dbContext)
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
            var claimAccount = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Name);

            var player = _dbContext.Players.Where(a => a.Account == claimAccount.Value.ToLowerInvariant()).FirstOrDefault();

            return player;
        }

        [HttpGet("GetUsers")]
        public List<string> GetUsers()
        {
           return _dbContext.Players.Select(x=>x.Name).ToList();
        }

        [HttpPost("CreatePlayer")]
        public async Task<IActionResult> CreatePlayer([FromBody] PlayerName playerName)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claimAccount = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Name);

            var player = _dbContext.Players.Where(a => a.Account == claimAccount.Value.ToLowerInvariant()).FirstOrDefault();

            if (player != null)
            {

                return Conflict("Player already exist");
            }
            else
            {
                string nameLower = playerName.Name.ToLowerInvariant();
                var playerSameName = _dbContext.Players.Where(a => EF.Functions.ILike(a.Name, $"{nameLower}")).FirstOrDefault();

                var accountId = claimAccount.Value.ToLowerInvariant();
                var playerEntity = new Player()
                {
                    // register Account as lower
                    Account = accountId,
                    Name = playerName.Name
                };

                _dbContext.Players.Add(playerEntity);
                _dbContext.SaveChanges();


                return Ok(playerEntity);
            }
        }
    }
}
