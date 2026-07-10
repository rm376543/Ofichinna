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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
