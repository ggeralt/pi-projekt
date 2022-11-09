using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using WebApi.Model;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static User user = new User();

        private readonly IConfiguration configuration;

        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("registration")]
        public IActionResult Register(UserDto registration)
        {
            GeneratePasswordHash(registration.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.FirstName = registration.FirstName;
            user.LastName = registration.LastName;
            user.Email = registration.Email;
            user.PasswordHash = passwordHash; 
            user.PasswordSalt = passwordSalt;

            return Ok(user);
        }

        [HttpPost("login")]
        public IActionResult Login(UserLogin login) 
        {
            if (user.Email != login.Email || !VerifyPasswordHash(login.Password, user.PasswordHash, user.PasswordSalt))
            {
                return NotFound("Invalid email or password");
            }

            string token = GenerateToken(user);
            return Ok(token);
        }

        private string GenerateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Surname, user.LastName)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims, 
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void GeneratePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(user.PasswordSalt))
            {
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return hash.SequenceEqual(passwordHash);
            }
        }
    }
}
