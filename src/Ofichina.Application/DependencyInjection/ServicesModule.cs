using Microsoft.Extensions.DependencyInjection;

namespace Ofichina.Application.DependencyInjection;

/// <summary>
/// Módulo de registro de serviços da aplicação.
/// Registra serviços de domínio e aplicação.
/// </summary>
public static class ServicesModule
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Registra serviços específicos da aplicação aqui
        // Exemplo: services.AddScoped<IClienteService, ClienteService>();

        return services;
    }
}
