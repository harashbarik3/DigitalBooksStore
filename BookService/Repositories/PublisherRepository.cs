using CommonLib.Models;
using Microsoft.EntityFrameworkCore;
using UserServiceLib;

namespace BookService.Repositories
{
    public class PublisherRepository : IPublisherRepository
    {
        private readonly DigitalbookstoreContext _context;
        public PublisherRepository(DigitalbookstoreContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Publisher>> GetAllPublisherAsync()
        {
            return await _context.Publishers.ToListAsync();
        }

        public async Task<Publisher> AddPublisherAsync(Publisher publisher)
        {
            publisher.PublisherId = Guid.NewGuid();
            await _context.AddAsync(publisher);
            await _context.SaveChangesAsync();

            return publisher;
        }
    }
}
