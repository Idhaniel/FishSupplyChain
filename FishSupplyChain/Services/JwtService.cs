using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FishSupplyChain.Dtos;
using Microsoft.IdentityModel.Tokens;

namespace FishSupplyChain.Services
{
    public class JwtService(IConfiguration configuration)
    {
        public string CreateToken(CreateTokenForUserDto userInfo)
        {
            string? issuer = configuration["JWT-Config:Issuer"];
            string? audience = configuration["JWT-Config:Audience"];
            string? key = configuration["JWT-Config:Key"];
            var now = DateTimeOffset.UtcNow;
            var expires = now.AddMinutes(configuration.GetValue<int>("JWT-Config:LifetimeInMinutes"));

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
                    new Claim(ClaimTypes.Email, userInfo.Email),
                }),
                NotBefore = now.UtcDateTime,
                IssuedAt = now.UtcDateTime,
                Issuer = issuer,
                Audience = audience,
                Expires = expires.UtcDateTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }
    }
}
