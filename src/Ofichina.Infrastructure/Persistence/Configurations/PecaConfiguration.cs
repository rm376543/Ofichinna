using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração de mapeamento da entidade <see cref="Peca"/>.
/// </summary>
public sealed class PecaConfiguration : IEntityTypeConfiguration<Peca>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Peca> builder)
    {
        builder.ToTable("Pecas");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("PecaId");

        builder.Property(p => p.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Descricao)
            .HasMaxLength(500);

        builder.Property(p => p.Codigo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Valor)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(p => p.QuantidadeEstoque)
            .IsRequired();

        builder.HasIndex(p => p.Codigo)
            .IsUnique();
    }
}