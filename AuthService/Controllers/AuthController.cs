using AuthService.Data;
using AuthService.Models;
using AuthService.Models.Enum;
using AuthService.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequest register)
        {
            try
            {
                var user = new User
                {
                    Email = register.Email,
                    Password = CryptoUtil.EncryptPassword(register.Password),
                    Role = Roles.User,
                };

                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();

                return Ok("User Created");
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
            {
                return Conflict(new { Message = $"Email '{register.Email}' is already in use." });
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
            {
                return Conflict(new { Message = $"Email '{register.Email}' is already in use." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while registering the user.", Error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest login)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
            if (user == null)
                return BadRequest("Invalid credentials");

            var result = CryptoUtil.IsPasswordCorrect(login.Password, user.Password);
            if (result)
            {
                string token = GenerateToken(user, user.Role.ToString());
                return Ok(new { Token = token });
            }
            return BadRequest("Invalid credentials");
        }

        private string GenerateToken(User user, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Email, user.Email!),
                            new Claim(ClaimTypes.Role, role),
                        };
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public record AuthRequest(string Email, string Password);
    }
}
