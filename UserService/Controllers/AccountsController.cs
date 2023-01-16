using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class AccountsController
    {
        public string GetToken()
        {
            return DateTime.Now.ToString();
        }
    }
}
