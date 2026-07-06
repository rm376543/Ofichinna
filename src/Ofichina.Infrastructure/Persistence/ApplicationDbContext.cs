using Microsoft.EntityFrameworkCore;
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
    /// DbSet para a entidade Exemplo.
    /// </summary>
    public DbSet<Exemplo> Exemplos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações de entidades podem ser adicionadas aqui
        // Exemplo:
        // modelBuilder.ApplyConfiguration(new ExemploConfiguration());
    }
}
