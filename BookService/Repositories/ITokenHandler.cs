using CommonLib.Models;
namespace BookService.Repositories
{
    public interface ITokenHandler
    {
        Task<string> CreateTokenAsync(string key, string issuer, IEnumerable<string> audience, string username);
    }
}
