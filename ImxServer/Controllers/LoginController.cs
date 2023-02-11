using Imx.Sdk;
using ImxServer.Models;
using ImxServer.Services;
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
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly ILogger<LoginController> _logger;
        private const string message = "Welcome to the Imxverse, sign this message to authenticate {0}";
        private const string cacheConnectionKey = "GetConnection_{0}";
        private readonly IMintService _mintService;

        public LoginController(ILogger<LoginController> logger, IConfiguration config, IMemoryCache cache, IMintService mintService)
        {
            _logger = logger;
            _config = config;
            _cache = cache;
            _mintService = mintService;
        }

        [AllowAnonymous]
        [HttpGet("Mint")]
        public async Task Mint()
        {
            Client client = new Client(new Config()
            {
                Environment = EnvironmentSelector.Sandbox // Or EnvironmentSelector.Mainnet
            });
            var result = await client.MintsApi.ListMintsAsync(1, orderBy: "token_id", direction: "desc", tokenAddress: _config["ContractAddress"]);
            var last = result.Result.FirstOrDefault();
            int tokenId = last != null ? int.Parse(last.Token.Data.TokenId) + 1 : 1;
            await _mintService.Mint(tokenId, "0x38134d792AF74bBA3Fb7d23713b9Bc913dFBdeaE");
        }

        [AllowAnonymous]
        [HttpGet("RequestConnection")]
        public MessageVM RequestConnection(string account)
        {
            var connectVm = RequestEntry(account);

            return new MessageVM { Account = connectVm.Account, Message = string.Format(message, connectVm.Nonce) };
        }

        [AllowAnonymous]
        [HttpPost("CreateToken")]
        public async Task<IActionResult> CreateToken([FromBody] LoginVM login)
        {
            var user = await Authenticate(login);

            if (user != null)
            {
                var tokenString = BuildToken(user.Account);
                return Ok(new { token = tokenString });
            }

            return Unauthorized();
        }

        private ConnectionVM CheckEntry(string account)
        {
            return GetCacheConnection(account);
        }

        private ConnectionVM RequestEntry(string account)
        {
            return GetCacheConnection(account);
        }

        private ConnectionVM GetCacheConnection(string account)
        {
            var key = string.Format(cacheConnectionKey, account);
            return _cache.GetOrCreate(key, (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3);
                var test = new ConnectionVM();
                return new ConnectionVM { Account = account, DateTime = DateTime.Now, Nonce = Guid.NewGuid() };
            });
        }

        private void ResetCache(string account)
        {
            var key = string.Format(cacheConnectionKey, account);
            _cache.Remove(cacheConnectionKey);
        }

        private string BuildToken(string account)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Name, account),
            new Claim(JwtRegisteredClaimNames.Email, account),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             new Claim(JwtRegisteredClaimNames.Iss, _config["Jwt:Issuer"]),
              new Claim(JwtRegisteredClaimNames.Aud, _config["Jwt:Audience"])
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
             claims: claims,
             issuer: _config["Jwt:Issuer"],
             audience: _config["Jwt:Audience"],
              expires: DateTime.Now.AddMinutes(90),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<UserVM> Authenticate(LoginVM login)
        {
            // TODO: This method will authenticate the user recovering his Ethereum address through underlaying offline ecrecover method.

            var connectVM = CheckEntry(login.Signer);
            var messageVM = string.Format(message, connectVM.Nonce);

            // delete from cache to revoke reuse
            ResetCache(login.Signer);

            if (messageVM != login.Message)
            {
                _cache.Remove(cacheConnectionKey);
                throw new Exception("Authentification expired retry");
            }


            var messageToVerify = string.Format(message, connectVM.Nonce);
            UserVM user = null;

            var signer = new EthereumMessageSigner();
            var account = signer.EncodeUTF8AndEcRecover(messageToVerify, login.Signature);

            if (account.ToLower().Equals(login.Signer.ToLower()))
            {
                // read user from DB or create a new one
                // for now we fake a new user
                user = new UserVM { Account = account, Name = string.Empty, Email = string.Empty };
            }
            else
            {
                throw new Exception("Impossible to valid your auth, signature incorrect");
            }

            return user;
        }
    }
}
