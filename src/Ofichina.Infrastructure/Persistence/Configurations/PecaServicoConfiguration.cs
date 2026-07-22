using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração EF Core para a entidade <see cref="PecaServico"/>.
/// </summary>
public sealed class PecaServicoConfiguration : IEntityTypeConfiguration<PecaServico>
{
    public void Configure(EntityTypeBuilder<PecaServico> builder)
    {
        builder.ToTable("PecaServico");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PecaId)
            .IsRequired();

        builder.Property(x => x.ServicoId)
            .IsRequired();

        builder.Property(x => x.Quantidade)
            .IsRequired();

        builder.Property(x => x.Utilizada)
            .IsRequired();

        builder.Property(x => x.DataUtilizacao);

        builder.HasIndex(x => new { x.ServicoId, x.PecaId })
            .IsUnique()
            .HasFilter("[DeletedAt] IS NULL");

        builder.HasIndex(x => x.ServicoId);

        builder.HasIndex(x => x.PecaId);

        builder.HasOne(x => x.Peca)
            .WithMany()
            .HasForeignKey(x => x.PecaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Servico)
            .WithMany(x => x.Pecas)
            .HasForeignKey(x => x.ServicoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}