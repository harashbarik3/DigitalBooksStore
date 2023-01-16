using CommonLib.Models;
using DigitalBookStore.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
//using NuGet.Protocol;
using System.Net.Http.Headers;
using System.Security.Claims;
namespace DigitalBookStore.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor _context;

        public AccountController(IHttpContextAccessor context)
        {
            _context = context;
        }
        public string BaseUrl = "https://localhost:7128/";

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

      
        public ActionResult Register(Register register)
        {
            // if(Request.HttpMethod=="Post")
            try
            {
                if (ModelState.IsValid)
                {
                    var uri = BaseUrl + "gateway/users/add";
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        User userRegister = new User
                        {
                            FirstName = register.FirstName,
                            LastName = register.LastName,
                            UserName = register.UserName,
                            RoleId = System.Guid.NewGuid(),
                            Password = register.Password,
                            Email = register.Email,
                            UserType = register.UserType,
                            Role = new Role { RoleId = System.Guid.NewGuid(), RoleName = register.UserType }
                        };
                        //Create User

                        var createUser = client.PostAsJsonAsync<User>(uri, userRegister);
                        createUser.Wait();

                        var result = createUser.Result;

                        if (result.IsSuccessStatusCode)
                        {
                            // TempData["successMessage"] = "User has been created successfully!";
                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {
                            return View();
                        }

                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        
        public ActionResult Login(DigitalBookStore.Web.Models.Login login)
        {
            // if(Request.HttpMethod=="Post")
            try
            {
                var uri = BaseUrl + "gateway/users/Login";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    CommonLib.Models.Login userlogin = new CommonLib.Models.Login()
                    {
                        Userid = login.Username,
                        Password = login.Password,
                    };

                    //Create User
                    var loggedUser = client.PostAsJsonAsync(uri, userlogin).ContinueWith(async x => await x.Result.Content.ReadAsStringAsync());
                    loggedUser.Wait();

                    var response = loggedUser.Result;
                    var responseRes = response.Result;
                    var tokenWithUser = JsonConvert.DeserializeObject<LoggedInUser>(responseRes);

                    var tokenres = tokenWithUser.token;
                    var users = tokenWithUser.user;

                    if (tokenres != null || users != null)
                    {
                        _context.HttpContext.Session.SetString("token", tokenres.ToString());
                        _context.HttpContext.Session.SetString("userId", users.UserId.ToString());
                        _context.HttpContext.Session.SetString("user", users.ToString());
                        _context.HttpContext.Session.SetString("userRole", users.UserType.ToString());
                        _context.HttpContext.Session.SetString("userEmail", users.Email.ToString());
                    }


                    if (tokenWithUser.user != null)
                    {
                        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, login.Username) }, CookieAuthenticationDefaults.AuthenticationScheme);

                        var principal = new ClaimsPrincipal(identity);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        HttpContext.Session.SetString("Username", login.Username);

                        if (tokenWithUser.user.UserType.ToLower() == "sysadmin")
                        {
                            return RedirectToAction("Index", "SubscriptionDashboard");
                        }

                        return RedirectToAction("Index", "Book", new { @token = tokenres });
                    }
                    else
                    {
                        TempData["errorPassword"] = "Invalid password!";
                        return View(login);
                    }

                }
            }
            catch (Exception ex)
            {
                TempData["errorUsername"] = "Username not found!";
                return View(login);
            }
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
