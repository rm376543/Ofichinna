using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;
namespace Ofichina.Infrastructure.Persistence.Configurations;

public class UsuarioPerfilConfiguration : IEntityTypeConfiguration<UsuarioPerfil>
{
    public void Configure(EntityTypeBuilder<UsuarioPerfil> builder)
    {
        builder.ToTable("UsuariosPerfis");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("UsuarioPerfilId");

        builder.HasIndex(x => new { x.UsuarioId, x.PerfilId })
            .IsUnique();
    }
}