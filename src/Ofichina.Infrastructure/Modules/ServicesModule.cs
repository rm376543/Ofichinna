using Microsoft.Extensions.DependencyInjection;
using Ofichinna.Authentication.Abstractions;
using Ofichina.Infrastructure.Services;

namespace Ofichina.Infrastructure.DependencyInjection;

/// <summary>
/// Módulo de registro de serviços de infraestrutura.
/// Registra serviços específicos da camada de infraestrutura.
/// Exemplos: Email, SMS, Storage, etc.
/// </summary>
public static class InfrastructureServicesModule
{
    /// <summary>
    /// Registra os serviços de infraestrutura.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddScoped<IPerfilAutorizacaoService, PerfilAutorizacaoService>();

        return services;
    }
}

