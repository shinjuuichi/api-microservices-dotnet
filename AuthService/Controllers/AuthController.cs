
using AuthService.Service;
using JwtAuthenticationManager;
using JwtAuthenticationManager.Data;
using JwtAuthenticationManager.Dto;
using JwtAuthenticationManager.Models;
using JwtAuthenticationManager.Models.Enum;
using Microsoft.AspNetCore.Authorization;
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
    public class AuthController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthenticationRequest registerRequest)
        {
            try
            {
                var result = await _userService.RegisterUserAsync(registerRequest);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest loginRequest)
        {
            var response = await _userService.AuthenticateUserAsync(loginRequest);
            if (response == null)
            {
                return BadRequest("Invalid credentials");
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.Claims.Any())
            {
                return Unauthorized(new { Message = "Invalid or expired token" });
            }

            var email = identity.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userService.GetUserByEmailAsync(email!);

            if (user == null)
                return NotFound(new { Message = "User not found" });

            return Ok(new { Id = user.Id, Email = user.Email, Role = user.Role });
        }
    }
}
