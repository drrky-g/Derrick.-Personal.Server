using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Derrick.Personal.Server.Repository.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Derrick.Personal.Server.Repository.Services;

public class AdminTokenGeneratorService : IAdminTokenGenerator
{
    private readonly string _adminToken;
    
    public AdminTokenGeneratorService(string adminToken) => 
        _adminToken = adminToken;
    
    public string GenerateToken(string email)
    {
        var handler = new JwtSecurityTokenHandler();
        
        byte[] key = Encoding.UTF8.GetBytes(_adminToken).ToArray();
        
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, $"{Guid.NewGuid()}"),
            new(JwtRegisteredClaimNames.Sub, email),
            new(JwtRegisteredClaimNames.Email, email)
        };

        var descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = "https://localhost:44310/",
            Audience = "https://localhost:44310/",
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = handler.CreateToken(descriptor);
        return handler.WriteToken(token);

    }
}