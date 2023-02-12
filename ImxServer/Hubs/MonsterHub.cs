using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Imx.Sdk;
using ImxServer.Controllers;
using ImxServer.Models;
using ImxServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ImxServer.Hubs
{
    [Authorize]
    public class MonsterHub : Hub
    {
        private IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly ILogger<MonsterHub> _logger;
        private readonly GameContext _dbContext;
        private readonly IMintService _mintService;

        public MonsterHub(ILogger<MonsterHub> logger, IConfiguration config, IMemoryCache cache, GameContext dbContext, IMintService mintService)
        {
            _logger = logger;
            _config = config;
            _cache = cache;
            _dbContext = dbContext;
            _mintService = mintService;
        }

        public async Task MintMonster(string name, int level, List<string> moves)
        {

            var claimsIdentity = Context.User.Identity as ClaimsIdentity;
            var claimAccount = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Name);

            Client client = new Client(new Config()
            {
                Environment = EnvironmentSelector.Sandbox // Or EnvironmentSelector.Mainnet
            });
            var result = await client.MintsApi.ListMintsAsync(1, orderBy: "token_id", direction: "desc", tokenAddress: _config["ContractAddress"]);
            var last = result.Result.FirstOrDefault();
            // get next tokenId
            int tokenId = last != null ? int.Parse(last.Token.Data.TokenId) + 1 : 1;

            var getMonster = _dbContext.Monsters.Where(a => EF.Functions.ILike(a.Name, $"{name.ToLower()}")).FirstOrDefault();
            var getMoves = _dbContext.Moves.Where(m => moves.Contains(m.Name)).ToList();
            await _mintService.Mint(tokenId, claimAccount.Value, getMonster);



            var token = new Token()
            {
                Level = level,
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

            await GetMonsters();

        }

        public async Task UpdateMonster(int tokenId, int level,int exp, List<string> moves)
        {

            var claimsIdentity = Context.User.Identity as ClaimsIdentity;
            var claimAccount = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Name);

            var getMonster = _dbContext.Tokens.Where(a => a.TokenId == tokenId).FirstOrDefault();
            if (moves?.Count > 0)
            {
                var getMoves = _dbContext.Moves.Where(m => moves.Contains(m.Name)).ToList();
                getMoves.ForEach(m =>
                {
                    var moveMonster = new MonsterMove()
                    {
                        TokenId = tokenId,
                        MoveId = m.MoveId
                    };
                    _dbContext.MonsterMoves.Add(moveMonster);
                });
            }

            getMonster.Level = level;
            getMonster.Exp = exp;
           
            _dbContext.SaveChanges();

            await Clients.Caller.SendAsync("Monster updated", tokenId);

        }


        public async Task GetMonsters()
        {

            var claimsIdentity = Context.User.Identity as ClaimsIdentity;
            var claimAccount = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Name);

            Client client = new Client(new Config()
            {
                Environment = EnvironmentSelector.Sandbox // Or EnvironmentSelector.Mainnet
            });
            var result = await client.MintsApi.ListMintsAsync(50, orderBy: "token_id", direction: "asc", user:claimAccount.Value, tokenAddress: _config["ContractAddress"]);

            if (result.Result?.Count > 0)
            {
              var tokenIds =  result.Result.Select(x => int.Parse(x.Token.Data.TokenId));
              var monsters = _dbContext.Tokens.Where(x => tokenIds.Contains(x.TokenId)).Include(x=>x.Monster).ToList();
              await Clients.Caller.SendAsync("GetMonsters", monsters);
            }

            await Clients.Caller.SendAsync("GetMonsters", new List<Token>());

        }

    }
}

