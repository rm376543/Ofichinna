using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Ofichina.Authentication;

/// <summary>
/// Módulo de registro de Autenticacao do Sistema
/// Registra toda logica de autenticacao do sistema
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

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    Console.WriteLine("Evento: OnMessageReceived");

                    return Task.CompletedTask;
                },

                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine("Evento: OnAuthenticationFailed");

                    return Task.CompletedTask;
                },

                OnTokenValidated = context =>
                {
                    Console.WriteLine("Evento: OnAuthenticationFailed");

                    return Task.CompletedTask;
                },

                OnChallenge = context =>
                {
                    Console.WriteLine("Evento: OnChallenge");

                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}