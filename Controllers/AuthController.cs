using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoTEC_Server.Logic;
using CoTEC_Server.Logic.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoTEC_Server.Controllers
{
    public class AuthController : Controller
    {

        [HttpPost]
        public IActionResult Authenticate([FromBody] UserCred user)
        {

            if (user == null)
            {
                return BadRequest("Identification credentials must be provided");

            } else if (user.id == null || user.id.Trim().Equals(""))
            {
                return BadRequest("Identification code must be provided");

            } else if (user.pass == null || user.pass.Trim().Equals(""))
            {
                return BadRequest("Password must be provided");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.id.Trim()),
                new Claim(JwtRegisteredClaimNames.Iss, Constants.Issuer),
                //TODO: Insert correct role
                new Claim(Constants.RoleClaim, Constants.AdminRoleName)
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
