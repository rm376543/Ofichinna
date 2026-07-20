using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace Ofichina.Application.DependencyInjection;

/// <summary>
/// Módulo de registro dos handlers da aplicação usando MediatR.
/// Faz o escaneamento do assembly para localizar commands, queries e handlers.
/// </summary>
public static class HandlersModule
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(HandlersModule).Assembly));

        return services;
    }
}
