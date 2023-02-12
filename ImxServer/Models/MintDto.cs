using System;
namespace ImxServer.Models
{
    public class MintDto
    {
        public string auth_signature { get; set; }
        public string contract_address { get; set; }
        public List<RoyaltyDto> royalties { get; set; }
        public List<UserDto> users { get; set; }
    }

    public class MintInfo
    {

        public string tokenId { get; set; }
        public string userAddress { get; set; }
        public string monsterId { get; set; }
        public string name { get; set; }
    }

    public class RoyaltyDto
    {
        public int percentage { get; set; }
        public string recipient { get; set; }
    }

    public class TokenDto
    {
        public string blueprint { get; set; }
        public string id { get; set; }
        public List<RoyaltyDto> royalties { get; set; }
    }

    public class UserDto
    {
        public List<TokenDto> tokens { get; set; }
        public string user { get; set; }
    }

}

