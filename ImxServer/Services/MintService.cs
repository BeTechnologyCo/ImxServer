using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Text;
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


        public async Task Mint(int tokenId, string addressUser)
        {

            //Environment environment = new Environment
            //{
            //    BaseApiPath = "https://api.sandbox.x.immutable.com",
            //    EthereumRpc = "https://eth-goerli.alchemyapi.io/v2/",
            //    RegistrationContractAddress = "0x1C97Ada273C9A52253f463042f29117090Cd7D83",
            //    CoreContractAddress = "0x7917eDb51ecD6CdB3F9854c3cc593F33de10c623",
            //    ChainId = 5
            //};
            MintInfo infos = new MintInfo();
            infos.tokenId = tokenId.ToString();
            infos.userAddress = addressUser;
           

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

        //      // used chained with serializeEthSignature. serializeEthSignature(deserializeSignature(...))
        //      private deserializeSignature(sig: string, size = 64) :  {
        //          sig = encUtils.removeHexPrefix(sig);
        //          return {
        //          r: new BN(sig.substring(0, size), 'hex'),
        //  s: new BN(sig.substring(size, size * 2), 'hex'),
        //  recoveryParam: importRecoveryParam(sig.substring(size * 2, size * 2 + 2)),
        //};
        //      }

    }
}

