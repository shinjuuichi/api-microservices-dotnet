using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ApiGateway.Middlewares
{
    public class TokenCheckerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public TokenCheckerMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string requestPath = context.Request.Path.Value ?? string.Empty;

            if (requestPath.Contains("/api/v1/Auth", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader) ||
                string.IsNullOrWhiteSpace(authorizationHeader) ||
                !authorizationHeader.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                await RespondUnauthorized(context, "Unauthorized");
                return;
            }

            string token = authorizationHeader.ToString()["Bearer ".Length..].Trim();

            ClaimsPrincipal? claimsPrincipal = GetClaimPrincipal(token);
            if (claimsPrincipal == null)
            {
                await RespondUnauthorized(context, "Invalid token");
                return;
            }

            context.User = claimsPrincipal;
            await _next(context);
        }

        private ClaimsPrincipal? GetClaimPrincipal(string jwtToken)
        {
            string secretKey = _configuration["Jwt:Key"]!;
            string validIssuer = _configuration["Jwt:Issuer"]!;
            string validAudience = _configuration["Jwt:Audience"]!;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = true,
                ValidIssuer = validIssuer,
                ValidateAudience = true,
                ValidAudience = validAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero 
            };

            JwtSecurityTokenHandler tokenHandler = new();
            try
            {
                return tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out _);
            }
            catch (Exception)
            {
                return null; 
            }
        }

        private static async Task RespondUnauthorized(HttpContext context, string message)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
        }
    }
}
