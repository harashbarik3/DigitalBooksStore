using CommonLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;


namespace BookService.Repositories
{
    public class TokenHandler : ITokenHandler
    {
        private readonly IConfiguration _configuration;
        public TokenHandler(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        //public Task<string> CreateTokenAsync(User user)
        //{
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        //    //Create claimns

        //}

        private TimeSpan ExpiryDuration=new TimeSpan(20,30,0);
        public Task<string> CreateTokenAsync(string key,string issuer,IEnumerable<string> audience, string username)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName,username),
            };    

            claims.AddRange(audience.Select(aud => new Claim(JwtRegisteredClaimNames.Aud, aud)));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescription=new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: DateTime.Now.Add(ExpiryDuration),
                signingCredentials: credentials);
            
            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(tokenDescription));
        }
    }
}
