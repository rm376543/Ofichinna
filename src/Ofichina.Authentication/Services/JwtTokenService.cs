using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Ofichina.Contracts.Responses;
using Ofichina.Domain.Entities;
using Ofichina.Authentication.Abstractions;

namespace Ofichina.Authentication.Services;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<TokenJwtResponse> GerarTokenAsync(Usuario usuario, IReadOnlyCollection<string> perfis, CancellationToken cancellationToken = default)
    {
        var issuer = _configuration["Jwt:Issuer"] ?? "Ofichina";
        var audience = _configuration["Jwt:Audience"] ?? "Ofichina";
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key não configurada.");
        var expirationMinutes = int.TryParse(_configuration["Jwt:ExpirationMinutes"], out var minutes) ? minutes : 60;

        var expires = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, usuario.Email.Value),
            new(JwtRegisteredClaimNames.Email, usuario.Email.Value),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(perfis.Select(perfil => new Claim(ClaimTypes.Role, perfil)));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler();

        return Task.FromResult(new TokenJwtResponse
        {
            AccessToken = tokenHandler.WriteToken(token),
            ExpiraEm = expires
        });
    }
}