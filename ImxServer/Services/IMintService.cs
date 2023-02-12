using ImxServer.Models;

namespace ImxServer.Services
{
    public interface IMintService
    {
        Task Mint(int tokenId, string addressUser,Monster monster);
        Task Transfer(int tokenId, string addressUser);
    }

}