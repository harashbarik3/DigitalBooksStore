using CommonLib.Models;
using Microsoft.EntityFrameworkCore;
using UserServiceLib;

namespace BookService.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DigitalbookstoreContext _context;
        public CategoryRepository(DigitalbookstoreContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetAllCategoryAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            category.CategoryId = Guid.NewGuid();
            await _context.AddAsync(category);
            await _context.SaveChangesAsync();

            return category;
        }
    }
}
