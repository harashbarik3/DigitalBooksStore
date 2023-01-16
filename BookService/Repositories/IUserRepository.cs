using CommonLib.Models;

namespace BookService.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUserAsync();

        Task<User> AddUserAsync(User user);

        Task<LoggedInUser> SignIn(Login login);

        //Task<User> AuthenticateAsync(string username, string password);

    }
}
