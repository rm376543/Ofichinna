using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class PermissaoConfiguration : IEntityTypeConfiguration<Permissao>
{
    public void Configure(EntityTypeBuilder<Permissao> builder)
    {
        builder.ToTable("Permissoes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("PermissaoId");

        builder.Property(x => x.Codigo)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Descricao)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.Codigo)
            .IsUnique();

        builder.HasMany(x => x.PerfisPermissoes)
            .WithOne(x => x.Permissao)
            .HasForeignKey(x => x.PermissaoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}