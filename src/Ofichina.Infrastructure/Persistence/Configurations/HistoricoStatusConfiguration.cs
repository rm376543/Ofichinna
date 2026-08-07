using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public sealed class HistoricoStatusConfiguration : IEntityTypeConfiguration<HistoricoStatus>
{
    public void Configure(EntityTypeBuilder<HistoricoStatus> builder)
    {
        builder.ToTable("HistoricoStatus");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("HistoricoStatusId");

        builder.Property(x => x.EntidadeId).IsRequired();
        builder.Property(x => x.OrcamentoId);
        builder.Property(x => x.OrdemServicoId);
        builder.Property(x => x.TipoEntidade).HasMaxLength(100).IsRequired();
        builder.Property(x => x.StatusAnterior).HasMaxLength(40);
        builder.Property(x => x.StatusNovo).HasMaxLength(40).IsRequired();
        builder.Property(x => x.AlteradoEm).IsRequired();
        builder.Property(x => x.AlteradoPor);

        builder.HasIndex(x => new { x.TipoEntidade, x.EntidadeId });

        builder.HasOne(x => x.Orcamento)
            .WithMany()
            .HasForeignKey(x => x.OrcamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OrdemServico)
            .WithMany()
            .HasForeignKey(x => x.OrdemServicoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}