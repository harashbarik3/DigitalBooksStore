using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public string GetToken()
        {            
            return DateTime.Now.ToString();
        }
    }
}
