using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ImxServer.Controllers;
using ImxServer.Models;
using ImxServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
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

        public async Task MintMonster(int id)
        {
            if (id > 0 && id < 2)
            {
                var claimsIdentity = Context.User.Identity as ClaimsIdentity;
                var claimAccount = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Name);

                // you have choice between 2 monster
                await _mintService.Mint(1, claimAccount.Value);

                await Clients.Caller.SendAsync("Minted");
            }
        }
    }
}

