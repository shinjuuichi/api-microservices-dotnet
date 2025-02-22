namespace JwtAuthenticationManager.Dto
{
    public class AuthenticationResponse
    {
        public string Email { get; set; }
        public string JwtToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
