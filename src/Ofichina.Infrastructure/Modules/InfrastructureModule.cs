using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ofichina.Infrastructure.DependencyInjection;

/// <summary>
/// Módulo principal de injeção de dependências da infraestrutura.
/// Orquestra todos os módulos de infraestrutura necessários.
/// 
/// Módulos carregados:
/// - DatabaseModule: Configura Entity Framework Core e DbContext
/// - RepositoryModule: Registra repositórios genéricos
/// - InfrastructureServicesModule: Registra serviços de infraestrutura (Email, etc)
/// </summary>
public static class InfrastructureModule
{
    /// <summary>
    /// Registra todos os serviços de infraestrutura.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddDatabase(configuration);
        services.AddRepositories();
        services.AddInfrastructureServices();

        return services;
    }
}