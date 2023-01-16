using BookService.Repositories;
using CommonLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserServiceLib;

namespace BookService.Controllers
{
    [Route("api/v1/digitalbooks/[controller]")]
    [ApiController]
    public class RolesController : Controller
    {
        private readonly IRoleRepository _roleRepository;
        public RolesController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleRepository.GetAllRolesAsync();

            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> AddGetAllRole([FromBody] Role role)
        {
            var addrole = new Role()
            {
                RoleName = role.RoleName
            };
            var addroleResult = await _roleRepository.AddRoleAsync(addrole);

            return Ok(addroleResult);
        }
    }
}
