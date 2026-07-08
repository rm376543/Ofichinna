using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence.Seeds;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class UsuarioPerfilConfiguration : IEntityTypeConfiguration<UsuarioPerfil>
{
    public void Configure(EntityTypeBuilder<UsuarioPerfil> builder)
    {
        builder.ToTable("UsuariosPerfis");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.UsuarioId, x.PerfilId })
            .IsUnique();

        builder.HasData(new UsuarioPerfil(
            usuarioId: AuthSeed.AdminUsuarioId,
            perfilId: AuthSeed.AdminPerfilId)
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            CreatedAt = new DateTime(2026, 7, 7, 21, 43, 51, 916, DateTimeKind.Utc).AddTicks(7067)
        });
    }
}