using Microsoft.EntityFrameworkCore;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence;

/// <summary>
/// Contexto de banco de dados da aplicação Ofichinna.
/// Define os DbSets para as entidades do domínio.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// DbSet para usuários autenticáveis.
    /// </summary>
    public DbSet<Usuario> Usuarios { get; set; } = null!;

    /// <summary>
    /// DbSet para perfis de autorização.
    /// </summary>
    public DbSet<Perfil> Perfis { get; set; } = null!;

    /// <summary>
    /// DbSet para vínculos entre usuários e perfis.
    /// </summary>
    public DbSet<UsuarioPerfil> UsuariosPerfis { get; set; } = null!;

    /// <summary>
    /// DbSet para pessoas.
    /// </summary>
    public DbSet<Pessoa> Pessoas { get; set; } = null!;

    /// <summary>
    /// DbSet para veículos.
    /// </summary>
    public DbSet<Veiculo> Veiculos { get; set; } = null!;

    /// <summary>
    /// DbSet para ordens de serviço.
    /// </summary>
    public DbSet<OrdemServico> OrdensServico { get; set; } = null!;

    /// <summary>
    /// DbSet para itens de serviço da ordem.
    /// </summary>
    public DbSet<ItemServico> ItensServico { get; set; } = null!;

    /// <summary>
    /// DbSet para serviços cadastrados no catálogo.
    /// </summary>
    public DbSet<Servico> Servicos { get; set; } = null!;

    /// <summary>
    /// DbSet para permissoes.
    /// </summary>
    public DbSet<Permissao> Permissoes { get; set; } = null!;

    /// <summary>
    /// DbSet para vínculos entre perfis e permissoes.
    /// </summary>
    public DbSet<PerfilPermissao> PerfisPermissoes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
