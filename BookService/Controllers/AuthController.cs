using BookService.Repositories;
using CommonLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;

namespace BookService.Controllers
{
    
    //public class AuthController : Controller
    //{
    //    private readonly IUserRepository _userRepository;
    //    private readonly ITokenHandler _tokenHandler;
    //    private readonly IConfiguration _configuration;
    //    public AuthController(IUserRepository userRepository, ITokenHandler tokenHandler, IConfiguration configuration)
    //    {
    //        _userRepository= userRepository;
    //        _tokenHandler= tokenHandler;
    //        _configuration = configuration;
    //    }


    //    [HttpPost]
    //    [Route("Signin")]
    //    public async Task<IActionResult> LoginAsync(Login Signin)
    //    {
    //        try
    //        {
    //            var authenticatedUser = await _userRepository.AuthenticateAsync(Signin.Userid, Signin.Password);

    //            if (authenticatedUser != null)
    //            {
    //                var token = await _tokenHandler.CreateTokenAsync(
    //                    _configuration["Jwt:Key"],
    //                    _configuration["Jwt:Issuer"],
    //                    new[] {
    //                    _configuration["Jwt:Aud1"],
    //                    _configuration["Jwt:Aud2"]
    //                    },
    //                    Signin.Userid);

                   
                    
    //                return Ok(token);

    //            }
    //            else
    //            {
    //                return NotFound();
    //            }
    //        }
    //        catch(Exception ex)
    //        {
    //            return BadRequest();
    //        }
            
           

           
    //    }
    //}
}
