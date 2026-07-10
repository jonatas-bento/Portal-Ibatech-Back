// Ibatech.Services/Security/JwtTokenGenerator.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ibatech.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Ibatech.Services.Security;

public sealed class JwtTokenGenerator(IConfiguration config)
{
    public (string Token, DateTime ExpiraEm) Gerar(Usuario usuario)
    {
        var secret = config["Jwt:Secret"]!;
        var issuer = config["Jwt:Issuer"]!;
        var audience = config["Jwt:Audience"]!;
        var horas = int.Parse(config["Jwt:ExpiresInHours"] ?? "8");
        var expiraEm = DateTime.UtcNow.AddHours(horas);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(JwtRegisteredClaimNames.Name,  usuario.NomeCompleto),
            new(ClaimTypes.Role,               usuario.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiraEm,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }
}