using BCrypt.Net;

namespace SharedLibrary
{
    public static class CryptoUtil
    {
        public static string EncryptPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool IsPasswordCorrect(string password, string? passwordHashed)
        {
            return passwordHashed != null && BCrypt.Net.BCrypt.Verify(password, passwordHashed);
        }
    }
}
