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
        // Registre aqui os repositórios específicos do domínio

        // Registra o repositório genérico
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Registra o Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IUsuarioAutenticacaoRepository, UsuarioAutenticacaoRepository>();

        services.AddScoped<IPerfilRepository, PerfilRepository>();

        services.AddScoped<IPerfilUsuarioRepository, PerfilUsuarioRepository>();

        services.AddScoped<IPessoaRepository, PessoaRepository>();

        services.AddScoped<IVeiculoRepository, VeiculoRepository>();

        services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();

        services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();

        services.AddScoped<IDiaDisponibilidadeRepository, DiaDisponibilidadeRepository>();
        services.AddScoped<IHorarioDisponibilidadeRepository, HorarioDisponibilidadeRepository>();
        services.AddScoped<IHorarioConsultorRepository, HorarioConsultorRepository>();

        return services;
    }
}



