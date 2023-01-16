using CommonLib.Models;

namespace BookService.Repositories
{
    public interface IPublisherRepository
    {
        Task<IEnumerable<Publisher>> GetAllPublisherAsync();
        Task<Publisher> AddPublisherAsync(Publisher publisher);
    }
}
