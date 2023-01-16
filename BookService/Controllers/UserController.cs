using BookService.Repositories;
using CommonLib.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Controllers
{
    [Route("api/v1/digitalbooks/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var rusers = await _userRepository.GetAllUserAsync(); ;

                return Ok(rusers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateUserAsync([FromBody] User user)
        {
            try
            {
                var addUser = new User()
                {
                    FirstName= user.FirstName,
                    LastName= user.LastName,
                    Email= user.Email,
                    UserName = user.UserName,
                    Password = user.Password,
                    RoleId = user.RoleId,
                    UserType= user.UserType,
                    
                };
                var userAdd = await _userRepository.AddUserAsync(addUser);

                return Ok(userAdd);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LogIn(Login login)
        {
            try
            {
                var token = await _userRepository.SignIn(login);

                if (token != null)
                {
                    return Ok(token);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Please check your username and password");
            }
            
        }
    }
}
