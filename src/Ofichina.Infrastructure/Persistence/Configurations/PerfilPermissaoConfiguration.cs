using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class PerfilPermissaoConfiguration : IEntityTypeConfiguration<PerfilPermissao>
{
    public void Configure(EntityTypeBuilder<PerfilPermissao> builder)
    {
        builder.ToTable("PerfisPermissoes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("PerfilPermissaoId");

        builder.HasIndex(x => new { x.PerfilId, x.PermissaoId })
            .IsUnique();
    }
}