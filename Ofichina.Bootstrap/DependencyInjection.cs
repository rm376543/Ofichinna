using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ofichina.Application.DependencyInjection;
using Ofichina.Infrastructure.DependencyInjection;

namespace Ofichina.Bootstrap;

public static class DependencyInjection
{
    public static IServiceCollection AddBootstrap(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);

        return services;
    }
}