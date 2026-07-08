using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Ofichinna.Authentication.Abstractions;
using Ofichinna.Authentication.Services;

namespace Ofichinna.Authentication.DependencyInjection;

/// <summary>
/// Registra os serviços de autenticação da aplicação.
/// </summary>
public static class AuthenticationServicesModule
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(AuthenticationServicesModule).Assembly, includeInternalTypes: true);
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<ISenhaHasher, SenhaHasherService>();

        return services;
    }
}