using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ofichina.Application.DependencyInjection;
using Ofichina.Infrastructure.DependencyInjection;

namespace Ofichina.Bootstrap;

public static class DependencyInjection
{
    public static IServiceCollection AddBootstrapMiddleware(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services;
    }
}