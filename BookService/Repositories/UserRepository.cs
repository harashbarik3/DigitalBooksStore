using CommonLib.Models;
using Microsoft.EntityFrameworkCore;
using UserServiceLib;

namespace BookService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DigitalbookstoreContext _context;
        private IConfiguration _config;
        public UserRepository(DigitalbookstoreContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public async Task<IEnumerable<User>> GetAllUserAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        public async Task<User> AddUserAsync(User user)
        {
            try
            {
                var role = await _context.Roles.Where(x => x.RoleName.ToLower() == user.UserType.ToLower()).FirstOrDefaultAsync();
                user.UserId = Guid.NewGuid();
                user.RoleId = role.RoleId;
                await _context.AddAsync(user);
                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                return null;
            }            
        }

        public async Task<LoggedInUser> SignIn(Login login)
        {
            string token = string.Empty;
            var tokenString = token;
            try
            {                
                var loggedUser = AuthenticateUser(login);
                if (loggedUser != null)
                {
                    tokenString = GenerateToken(loggedUser);


                    return new LoggedInUser
                    {
                        token = tokenString,
                        user = loggedUser
                    };
                }

                return new LoggedInUser
                {
                    token = tokenString,
                    user = loggedUser
                };
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        private  User AuthenticateUser(Login login)
        {
            var user = new User();
            try
            {
                user =  _context.Users.Where(x => x.UserName == login.Userid && x.Password == login.Password).FirstOrDefault();


                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private string GenerateToken(User login)
        {
            try
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,login.UserName),
                    new Claim(ClaimTypes.Role,login.UserType)
                };
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(_config["jwt:Issuer"],
                    _config["jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: credentials);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

    }
}
