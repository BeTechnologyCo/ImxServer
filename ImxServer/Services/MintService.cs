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
            var royalties = new RoyaltyDto()
            {
                percentage = 1,
                recipient = "0x670cAcf48B685eB1AF8dc73C58AAbd30aA35958E"
            };
            var mintInfo = new MintDto();
            mintInfo.auth_signature = string.Empty;
            mintInfo.contract_address = _config["ContractAddress"];
            mintInfo.royalties = new List<RoyaltyDto>();
            mintInfo.royalties.Add(royalties);
            mintInfo.users = new List<UserDto>();
            var user = new UserDto()
            {
                user = addressUser,
                tokens = new List<TokenDto>()
            };
            var token = new TokenDto()
            {
                id = tokenId.ToString(),
                royalties = new List<RoyaltyDto>(),
                blueprint = tokenId.ToString()
            };
            token.royalties.Add(royalties);
            user.tokens.Add(token);

            mintInfo.users.Add(user);

            var info = JsonConvert.SerializeObject(mintInfo);
            var utf8 = Encoding.UTF8;
            byte[] utfBytes = utf8.GetBytes(info);

            var key = _config["PrivateKey"];
            var digest = new KeccakDigest(256);
            digest.BlockUpdate(utfBytes, 0, utfBytes.Length);
            var calculatedHash = new byte[digest.GetByteLength()];
            digest.DoFinal(calculatedHash, 0);

            // byte[] msgHash = hashing.CalculateHash(info);

            var abiEncode = new ABIEncode();
            var abiValue = new ABIValue("string", info);
            var encodedValue = abiEncode.GetABIEncodedPacked(abiValue);

            var hashing = new Sha3Keccack();
            byte[] msgHash = hashing.CalculateHash(encodedValue);

            var signer1 = new EthereumMessageSigner();
            var ecKey = new EthECKey(key);



            var hash = hashing.CalculateHash(info);

            var msgSign = ecKey.SignAndCalculateV(StringToByteArray(hash));
            mintInfo.auth_signature = EthECDSASignature.CreateStringSignature(msgSign);

            //var hexPrivateKey = "0xae78c8b502571dba876742437f8bc78b689cf8518356c0921393d89caaf284ce";
            //var signingKey = new EthECKey(hexPrivateKey);
            //var hash = hashing.CalculateHash("bou");
            //var signature = signingKey.SignAndCalculateV(StringToByteArray(hash));
            //var eresult = "0x" + HexByteConvertorExtensions.ToHex(signature.R, false).PadLeft(64, '0') +
            //    HexByteConvertorExtensions.ToHex(signature.S, false).PadLeft(64, '0') +
            //    HexByteConvertorExtensions.ToHex(signature.V, false);

            var postInfo = new List<MintDto>();
            postInfo.Add(mintInfo);

            using (var client = new HttpClient())
            {
                var url = "https://api.sandbox.x.immutable.com/v2/mints";

                var response = await client.PostAsJsonAsync(url, postInfo);

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

