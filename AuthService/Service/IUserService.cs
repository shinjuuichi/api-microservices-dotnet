using JwtAuthenticationManager.Dto;
using JwtAuthenticationManager.Models;

namespace AuthService.Service
{
    public interface IUserService
    {
        Task<string> RegisterUserAsync(AuthenticationRequest registerRequest);
        Task<AuthenticationResponse?> AuthenticateUserAsync(AuthenticationRequest loginRequest);
        Task<User?> GetUserByEmailAsync(string email);
    }

}
