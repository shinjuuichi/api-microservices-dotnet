using JwtAuthenticationManager.Data;
using JwtAuthenticationManager.Dto;
using JwtAuthenticationManager.Models.Enum;
using JwtAuthenticationManager.Models;
using JwtAuthenticationManager;
using SharedLibrary;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Service
{
    public class UserService(UserDbContext dbContext, JwtTokenHandler jwtTokenHandler) : IUserService
    {
        private readonly UserDbContext _dbContext = dbContext;
        private readonly JwtTokenHandler _jwtTokenHandler = jwtTokenHandler;

        public async Task<string> RegisterUserAsync(AuthenticationRequest registerRequest)
        {
            var existingUser = await _dbContext.Users.AnyAsync(u => u.Email == registerRequest.Email);
            if (existingUser)
            {
                throw new Exception($"Email '{registerRequest.Email}' is already in use.");
            }

            var user = new User
            {
                Email = registerRequest.Email,
                Password = CryptoUtil.EncryptPassword(registerRequest.Password),
                Role = Roles.User
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return "User Created";
        }

        public async Task<AuthenticationResponse?> AuthenticateUserAsync(AuthenticationRequest loginRequest)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
            if (user == null || !CryptoUtil.IsPasswordCorrect(loginRequest.Password, user.Password))
            {
                return null;
            }

            return _jwtTokenHandler.GenerateJwtToken(loginRequest);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
