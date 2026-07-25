using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Ofichina.Application.Abstractions.Authentication;
using Ofichina.Authentication.Services;

namespace Ofichina.Authentication.DependencyInjection;

/// <summary>
/// Registra os serviços de autenticação da aplicação.
/// </summary>
public static class AuthenticationServicesModule
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(AuthenticationServicesModule).Assembly, includeInternalTypes: true);
        services.AddHttpContextAccessor();
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();
        services.AddScoped<IUsuarioAtualService, UsuarioAtualService>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<ISenhaHasher, SenhaHasherService>();

        return services;
    }
}