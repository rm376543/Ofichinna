using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ofichina.Infrastructure.Persistence;

namespace Ofichina.Infrastructure.DependencyInjection;

/// <summary>
/// Módulo de configuração do banco de dados.
/// Configura Entity Framework Core com SQL Server.
/// </summary>
public static class DatabaseModule
{
    /// <summary>
    /// Registra o contexto de banco de dados da aplicação.
    /// </summary>
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "A connection string 'DefaultConnection' é necessária e não pode ser nula ou vazia. " +
                "Verifique o arquivo appsettings.json do seu respectivo ambiente.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}

