using CommonLib.Models;

namespace BookService.Repositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> AddRoleAsync(Role roles);
    }
}
