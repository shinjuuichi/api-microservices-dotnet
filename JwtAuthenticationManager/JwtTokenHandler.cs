using JwtAuthenticationManager.Data;
using JwtAuthenticationManager.Dto;
using JwtAuthenticationManager.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthenticationManager
{
    public class JwtTokenHandler
    {
        public const string JWT_SECURITY_KEY = "yPkCqn4kSWLtaJwXvN2jGzpQRyTZ3gdwadlmawdmkawdawkdnkpawnacOPCPANkkacaeonncpapPNOPXkt7FeBJP";
        private const int JWT_TOKEN_VALIDITY_MINS = 20;
        private readonly UserDbContext _context;

        public JwtTokenHandler(UserDbContext context)
        {
            _context = context;
        }

        public AuthenticationResponse? GenerateJwtToken(AuthenticationRequest authenticationRequest)
        {
            if (string.IsNullOrWhiteSpace(authenticationRequest.Email) || string.IsNullOrWhiteSpace(authenticationRequest.Password))
                return null;

            var findUser = _context.Users.FirstOrDefault(u => u.Email == authenticationRequest.Email);

            if (findUser == null) return null;

            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(JWT_TOKEN_VALIDITY_MINS);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, findUser.Id.ToString()),
                new Claim("Role", findUser.Role.ToString()),
                new Claim(ClaimTypes.Email, findUser.Email ?? string.Empty),
            };

            var claimsIdentity = new ClaimsIdentity(claims);

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = signingCredentials
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return new AuthenticationResponse
            {
                Email = findUser.Email,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds,
                JwtToken = token
            };
        }
    }
}
