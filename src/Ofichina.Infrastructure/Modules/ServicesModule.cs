using Microsoft.Extensions.DependencyInjection;

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
        // Registre aqui os serviços de infraestrutura
        // Exemplo:
        // services.AddScoped<IEmailService, EmailService>();
        // services.AddScoped<ISmsService, SmsService>();

        return services;
    }
}

