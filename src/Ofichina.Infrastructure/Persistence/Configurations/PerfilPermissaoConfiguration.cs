using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class PerfilPermissaoConfiguration : IEntityTypeConfiguration<PerfilPermissao>
{
    public void Configure(EntityTypeBuilder<PerfilPermissao> builder)
    {
        builder.ToTable("PerfilPermissao");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.PerfilId, x.PermissaoId })
            .IsUnique();
    }
}