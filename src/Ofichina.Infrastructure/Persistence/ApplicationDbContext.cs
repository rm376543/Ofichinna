using System.Reflection;
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
    /// DbSet para agendamentos.
    /// </summary>
    public DbSet<Agendamento> Agendamentos { get; set; } = null!;

    /// <summary>
    /// DbSet para dias de disponibilidade.
    /// </summary>
    public DbSet<DiaDisponibilidade> DiasDisponibilidade { get; set; } = null!;

    /// <summary>
    /// DbSet para horários de disponibilidade.
    /// </summary>
    public DbSet<HorarioDisponibilidade> HorariosDisponibilidade { get; set; } = null!;

    /// <summary>
    /// DbSet para vínculos entre dias e horários disponíveis.
    /// </summary>
    public DbSet<DiaHorarioDisponibilidade> DiasHorariosDisponibilidade { get; set; } = null!;

    /// <summary>
    /// DbSet para vínculos entre horários disponíveis e consultores.
    /// </summary>
    public DbSet<HorarioConsultor> HorariosConsultores { get; set; } = null!;

    /// <summary>
    /// DbSet para itens de serviço da ordem.
    /// </summary>
    public DbSet<ItemServico> ItensServico { get; set; } = null!;

    /// <summary>
    /// DbSet para serviços cadastrados no catálogo.
    /// </summary>
    public DbSet<Servico> Servicos { get; set; } = null!;

    /// <summary>
    /// DbSet para peças cadastradas no catálogo.
    /// </summary>
    public DbSet<Peca> Pecas { get; set; } = null!;

    /// <summary>
    /// DbSet para permissoes.
    /// </summary>
    public DbSet<Permissao> Permissoes { get; set; } = null!;

    /// <summary>
    /// DbSet para vínculos entre perfis e permissoes.
    /// </summary>
    public DbSet<PerfilPermissao> PerfisPermissoes { get; set; } = null!;
    public object ItemServicos { get; internal set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        var method = typeof(ApplicationDbContext)
            .GetMethod(nameof(ApplySoftDeleteQueryFilter), BindingFlags.NonPublic | BindingFlags.Static);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.ClrType is null || !typeof(Entity).IsAssignableFrom(entityType.ClrType) || entityType.ClrType == typeof(Entity))
            {
                continue;
            }

            method?.MakeGenericMethod(entityType.ClrType)
                .Invoke(null, [modelBuilder]);
        }
    }

    private static void ApplySoftDeleteQueryFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : Entity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(entity => entity.DeletedAt == null);
    }
}
