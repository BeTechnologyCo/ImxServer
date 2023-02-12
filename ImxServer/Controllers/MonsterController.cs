using Imx.Sdk;
using ImxServer.Models;
using ImxServer.Services;
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
    public class MonsterController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly ILogger<MonsterController> _logger;
        private readonly GameContext _dbContext;
        private readonly IMintService _mintService;


        public MonsterController(ILogger<MonsterController> logger, IConfiguration config, IMemoryCache cache, GameContext dbContext, IMintService mintService)
        {
            _logger = logger;
            _config = config;
            _cache = cache;
            _dbContext = dbContext;
            _mintService = mintService;
        }

        [HttpPost("MintMonster")]
        public async Task<List<Token>> MintMonster([FromBody] AddMonsterDto monsterDto)
        {

            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claimAccount = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Name);

            Client client = new Client(new Config()
            {
                Environment = EnvironmentSelector.Sandbox // Or EnvironmentSelector.Mainnet
            });
            var result = await client.MintsApi.ListMintsAsync(1, orderBy: "token_id", direction: "desc", tokenAddress: _config["ContractAddress"]);
            var last = result.Result.FirstOrDefault();
            // get next tokenId
            int tokenId = last != null ? int.Parse(last.Token.Data.TokenId) + 1 : 1;

            var getMonster = _dbContext.Monsters.Where(a => EF.Functions.ILike(a.Name, $"{monsterDto.Name.ToLower()}")).FirstOrDefault();
            var getMoves = _dbContext.Moves.Where(m => monsterDto.Moves.Contains(m.Name)).ToList();
            await _mintService.Mint(tokenId, claimAccount.Value, getMonster);



            var token = new Token()
            {
                Level = monsterDto.Level,
                MonsterId = getMonster.MonsterId,
                Exp = 0,
                TokenId = tokenId
            };

            _dbContext.Tokens.Add(token);

            getMoves.ForEach(m =>
            {
                var moveMonster = new MonsterMove()
                {
                    TokenId = tokenId,
                    MoveId = m.MoveId
                };
                _dbContext.MonsterMoves.Add(moveMonster);
            });
            _dbContext.SaveChanges();
            //await Clients.Caller.SendAsync("Minted");

          return  await GetMonsters();

        }

        [HttpPost("UpdateMonster")]
        public async Task<bool> UpdateMonster([FromBody] UpdateMonsterDto monsterDto)
        {

            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claimAccount = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Name);

            var getMonster = _dbContext.Tokens.Where(a => a.TokenId == monsterDto.TokenId).FirstOrDefault();
            if (monsterDto.Moves?.Count > 0)
            {
                var getMoves = _dbContext.Moves.Where(m => monsterDto.Moves.Contains(m.Name)).ToList();
                getMoves.ForEach(m =>
                {
                    var moveMonster = new MonsterMove()
                    {
                        TokenId = monsterDto.TokenId,
                        MoveId = m.MoveId
                    };
                    _dbContext.MonsterMoves.Add(moveMonster);
                });
            }

            getMonster.Level = monsterDto.Level;
            getMonster.Exp = monsterDto.Exp;

            _dbContext.SaveChanges();

            return true;
        }


        [HttpGet("GetMonsters")]
        public async Task<List<Token>> GetMonsters()
        {

            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claimAccount = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Name);

            Client client = new Client(new Config()
            {
                Environment = EnvironmentSelector.Sandbox // Or EnvironmentSelector.Mainnet
            });
            var result = await client.MintsApi.ListMintsAsync(50, orderBy: "token_id", direction: "asc", user: claimAccount.Value, tokenAddress: _config["ContractAddress"]);

            if (result.Result?.Count > 0)
            {
                var tokenIds = result.Result.Select(x => int.Parse(x.Token.Data.TokenId));
                var monsters = _dbContext.Tokens.Where(x => tokenIds.Contains(x.TokenId)).Include(x => x.Monster).ToList();
                return monsters;
            }

         return new List<Token>();

        }
    }
}
