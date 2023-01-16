using CommonLib.Models;

namespace BookService.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoryAsync();
        Task<Category> AddCategoryAsync(Category category);
    }
}
