using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ofichina.Application.DependencyInjection;
using Ofichina.Authentication.DependencyInjection;
using Ofichina.Infrastructure.DependencyInjection;
using Ofichina.Authentication;

namespace Ofichina.Bootstrap;

public static class DependencyInjection
{
    public static IServiceCollection AddBootstrapMiddleware(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthorizationModule();
        services.AddAuthenticationModules(configuration);
        services.AddAuthenticationServices();
        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services;
    }
}