using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoTEC_Server.Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoTEC_Server.Controllers
{
    public class AuthController : Controller
    {

        [HttpGet]
        public IActionResult Authenticate(string id_code, string password)
        {

            if (id_code == null || password == null)
            {
                return BadRequest("Identification code or password can't be empty");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, id_code.Trim()),
                new Claim(JwtRegisteredClaimNames.Iss, Constants.Issuer),
                //TODO: Insert correct role
                new Claim(Constants.RoleClaim, "Role")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Constants.key));


            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token;

            //TODO: Insert expiration days by role type
            if (true)
            {
                token = new JwtSecurityToken(
                issuer: Constants.Issuer,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(Constants.AdminExpireTimeHours),
                signingCredentials: signingCredentials);

            }
            else
            {
                token = new JwtSecurityToken(
                issuer: Constants.Issuer,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(Constants.HospitalWorkerExpireTimeHours),
                signingCredentials: signingCredentials);
            }


            var tokenR = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { access_token = tokenR });

        }

        
    }
}
