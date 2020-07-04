using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoTEC_Server.DBModels;
using CoTEC_Server.Logic;
using CoTEC_Server.Logic.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoTEC_Server.Controllers
{
    /// <summary>
    /// This class is for control the user authentication, it generates the JWT token used to give permissions
    /// </summary>
    public class AuthController : Controller
    {
        private CoTEC_DBContext db;

        public AuthController(CoTEC_DBContext dBContext)
        {
            db = dBContext;
        }

        [Route("Auth")]
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] UserCred user)
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

            var dbUser = await db.Staff.Where(s => s.IdCode.Equals(user.id.Trim()) && s.Password.Equals(user.pass)).FirstOrDefaultAsync();
            
            if (dbUser is null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, dbUser.FirstName + " " + dbUser.LastName),
                new Claim(Constants.RoleClaim, dbUser.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Constants.key));


            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token;

            if (dbUser.Role.Equals(Constants.AdminRoleName))
            {
                token = new JwtSecurityToken(
                issuer: Constants.Issuer,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(Constants.AdminExpireTimeHours),
                signingCredentials: signingCredentials);

            }
            else if (dbUser.Role.Equals(Constants.HealthCenterRoleName))
            {
                token = new JwtSecurityToken(
                issuer: Constants.Issuer,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(Constants.HospitalWorkerExpireTimeHours),
                signingCredentials: signingCredentials);
            }
            else
            {
                StringBuilder s = new StringBuilder();
                s.AppendFormat("User role ({0}) is not recognized", dbUser.Role);
                return Unauthorized(s.ToString());
            }


            var tokenR = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { access_token = tokenR, role = dbUser.Role });

        }
    }
}
