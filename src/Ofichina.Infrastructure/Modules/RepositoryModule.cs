using Microsoft.Extensions.DependencyInjection;
using Ofichina.Domain.Interfaces;
using Ofichina.Infrastructure.Repositories;
using Ofichina.Authentication.Abstractions;

namespace Ofichina.Infrastructure.DependencyInjection;

/// <summary>
/// Módulo de registro de repositórios.
/// Registra implementações de repositórios específicos do domínio.
/// </summary>
public static class RepositoryModule
{
    /// <summary>
    /// Registra os repositórios da aplicação.
    /// </summary>
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        // Registra o repositório genérico
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Registra o Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IUsuarioAutenticacaoRepository, UsuarioAutenticacaoRepository>();

        // Registre aqui os repositórios específicos do domínio
        services.AddScoped<IExemploRepository, ExemploRepository>();

        return services;
    }
}



