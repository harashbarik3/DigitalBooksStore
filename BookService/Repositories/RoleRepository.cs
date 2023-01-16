using CommonLib.Models;
using Microsoft.EntityFrameworkCore;
using UserServiceLib;

namespace BookService.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DigitalbookstoreContext _context;
        public RoleRepository(DigitalbookstoreContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role> AddRoleAsync(Role roles)
        {
            roles.RoleId = Guid.NewGuid();
            await _context.AddAsync(roles);
            await _context.SaveChangesAsync();

            return roles;
        }
    }
}
