using Microsoft.Extensions.DependencyInjection;

namespace Ofichina.Application.DependencyInjection;

/// <summary>
/// Módulo de registro de handlers (CQRS) da aplicação.
/// Registra todos os handlers de commands e queries.
/// </summary>
public static class HandlersModule
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        // Aqui podem ser registrados handlers de CQRS ou mediadores
        // Exemplo: services.AddMediatR(typeof(HandlersModule));

        return services;
    }
}
