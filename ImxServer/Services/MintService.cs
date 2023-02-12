using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Text;
using System.Threading;
using Imx.Sdk;
using ImxServer.Controllers;
using ImxServer.Models;
using Microsoft.Extensions.Caching.Memory;
using Nethereum.ABI;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Util;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Digests;

namespace ImxServer.Services
{
    public class MintService : IMintService
    {
        private IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly ILogger<MintService> _logger;
        private readonly GameContext _dbContext;

        public MintService(ILogger<MintService> logger, IConfiguration config, IMemoryCache cache, GameContext dbContext)
        {
            _logger = logger;
            _config = config;
            _cache = cache;
            _dbContext = dbContext;
        }


        public async Task Mint(int tokenId, string addressUser, Monster monster)
        {
            MintInfo infos = new MintInfo();
            infos.tokenId = tokenId.ToString();
            infos.userAddress = addressUser;
            infos.monsterId = monster.MonsterId.ToString();
            infos.name = monster.Name;


            using (var client = new HttpClient())
            {
                var url = "http://localhost:3000/api/mint";

                var response = await client.PostAsJsonAsync(url, infos);

                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();

                    Debug.WriteLine("token minted " + res);

                    //return res;
                }
                else
                {
                    throw new InvalidOperationException("Error server " + response.ReasonPhrase);
                }
            }
        }


        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private BigInteger importRecoveryParam(string v)
        {
            return !string.IsNullOrWhiteSpace(v)
              ? BigInteger.Parse(v).CompareTo(new BigInteger(27)) != -1
                ? BigInteger.Parse(v) - new BigInteger(27)
                : BigInteger.Parse(v) : BigInteger.Zero;
        }

        public async Task Transfer(int tokenId, string addressUser)
        {
            SendMonsterDto infos = new SendMonsterDto();
            infos.TokenId = tokenId.ToString();
            infos.ReceiverAddress = addressUser;

            using (var client = new HttpClient())
            {
                var url = "http://localhost:3000/api/transfer";

                var response = await client.PostAsJsonAsync(url, infos);

                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();

                    Debug.WriteLine("token trasnfered " + res);

                    //return res;
                }
                else
                {
                    throw new InvalidOperationException("Error server " + response.ReasonPhrase);
                }
            }
        }


    }
}

