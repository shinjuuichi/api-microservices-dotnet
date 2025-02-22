
using JwtAuthenticationManager;
using JwtAuthenticationManager.Data;
using JwtAuthenticationManager.Dto;
using JwtAuthenticationManager.Models;
using JwtAuthenticationManager.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController(UserDbContext _dbContext, JwtTokenHandler _jwtTokenHandler) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthenticationRequest registerRequest)
        {
            try
            {
                var user = new User
                {
                    Email = registerRequest.Email,
                    Password = CryptoUtil.EncryptPassword(registerRequest.Password),
                    Role = Roles.User,
                };

                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();

                return Ok("User Created");
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
            {
                return Conflict(new { Message = $"Email '{registerRequest.Email}' is already in use." });
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
            {
                return Conflict(new { Message = $"Email '{registerRequest.Email}' is already in use." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while registering the user.", Error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest loginRequest)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
            if (user == null)
                return BadRequest("Invalid credentials");

            var result = CryptoUtil.IsPasswordCorrect(loginRequest.Password, user.Password);
            if (result)
            {
                var response = _jwtTokenHandler.GenerateJwtToken(loginRequest);
                return Ok(response);
            }

            return BadRequest("Invalid credentials");
        }
    }
}
