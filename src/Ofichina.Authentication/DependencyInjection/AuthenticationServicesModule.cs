using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Ofichina.Application.Abstractions.Authentication.Service;
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
        services.AddScoped<IAuthService, AutenticacaoService>();
        services.AddScoped<IUserService, UsuarioAtualService>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasherService, SenhaHasherService>();

        return services;
    }
}