using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ofichina.Application.DependencyInjection;
using Ofichinna.Authentication.DependencyInjection;
using Ofichina.Infrastructure.DependencyInjection;
using Ofichinna.Authentication;

namespace Ofichina.Bootstrap;

public static class DependencyInjection
{
    public static IServiceCollection AddBootstrapMiddleware(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthenticationServices();
        services.AddAuthenticationModule(configuration);
        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services;
    }
}