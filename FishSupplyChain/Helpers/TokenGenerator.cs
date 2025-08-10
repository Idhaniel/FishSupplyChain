using System.Security.Cryptography;

namespace FishSupplyChain.Helpers
{
    public static class TokenGenerator
    {
        public static string GenerateSecureToken()
        {
            var randomBytes = new byte[64]; // 512-bit token
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            return Convert.ToBase64String(randomBytes); // URL-safe version? Use Base64UrlEncoder if needed
        }
    }
}

