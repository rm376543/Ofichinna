using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ofichina.Infrastructure.DependencyInjection;

namespace Ofichina.Application.DependencyInjection;

/// <summary>
/// Módulo principal de injeção de dependências da aplicação.
/// Orquestra todos os módulos (Application, Infrastructure) necessários.
/// 
/// Ordem de carregamento:
/// 1. ApplicationModule (este)
///    - ValidationModule (validações)
///    - HandlersModule (handlers CQRS)
///    - ServicesModule (serviços da aplicação)
/// 2. InfrastructureModule
///    - DatabaseModule (EF Core, DbContext)
///    - RepositoryModule (repositórios)
///    - ServicesModule (serviços de infraestrutura)
/// </summary>
public static class ApplicationModule
{
    /// <summary>
    /// Registra todos os serviços da aplicação e infraestrutura.
    /// </summary>
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // Registra módulos da Application Layer
        services.AddValidations();
        services.AddHandlers();
        services.AddApplicationServices();

        return services;
    }
}
