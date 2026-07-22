using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Ofichina.Authentication.Security;

namespace Ofichina.Authentication.DependencyInjection;

/// <summary>
/// Módulo de registro de Autenticacao do Sistema.
/// Registra toda logica de autenticacao do sistema.
/// </summary>
public static class AuthenticationModule
{
    public static IServiceCollection AddAuthenticationModules(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var issuer = configuration["Jwt:Issuer"] ?? "ofichinna";
        var audience = configuration["Jwt:Audience"] ?? "ofichinna";
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key não configurada.");

        services.AddTransient<LoggingJwtBearerEvents>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                NameClaimType = ClaimTypes.NameIdentifier,
                RoleClaimType = ClaimTypes.Role
            };

            options.EventsType = typeof(LoggingJwtBearerEvents);
        });

        return services;
    }
}