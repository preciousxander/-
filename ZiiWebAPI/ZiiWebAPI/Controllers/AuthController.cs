using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ClassDataBaseLibrary;

namespace ZiiWebAPI.Controllers
{
    [Route("auth/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private DataBase db = new DataBase("dataBase");

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            var findUser = await db.Set<User>().FirstOrDefaultAsync(u => u.Login == user.Login);

            if (findUser == null)
            {
                await db.Set<User>().AddAsync(user);
                await db.SaveChangesAsync();

                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            var findUser = await db.Set<User>().FirstOrDefaultAsync(u => u.Login == user.Login && u.Password == user.Password);

            if (findUser == null)
            {
                return Unauthorized();
            }
            else
            {
                //var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login) };
                //// создаем JWT-токен
                //var jwt = new JwtSecurityToken(
                //        issuer: AuthOptions.ISSUER,
                //        audience: AuthOptions.AUDIENCE,
                //        claims: claims,
                //        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(666)),
                //        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

                //var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                return Ok();
            }
        }
    }
}
