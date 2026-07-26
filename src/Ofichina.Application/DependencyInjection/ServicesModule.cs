using Microsoft.Extensions.DependencyInjection;
using Ofichina.Application.UseCases.OrdensServico.Services;

namespace Ofichina.Application.DependencyInjection;

/// <summary>
/// Módulo de registro de serviços da aplicação.
/// Registra serviços de domínio e aplicação.
/// </summary>
public static class ServicesModule
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICreateOrdemServicoService, CreateOrdemServicoService>();
        services.AddScoped<IOrdemServicoService, OrdemServicoService>();

        return services;
    }
}
