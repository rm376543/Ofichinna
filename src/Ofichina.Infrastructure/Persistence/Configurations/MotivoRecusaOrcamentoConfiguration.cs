using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public sealed class MotivoRecusaOrcamentoConfiguration : IEntityTypeConfiguration<MotivoRecusaOrcamento>
{
    public void Configure(EntityTypeBuilder<MotivoRecusaOrcamento> builder)
    {
        builder.ToTable("MotivosRecusaOrcamento");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrcamentoId).IsRequired();
        builder.Property(x => x.Descricao).HasMaxLength(1000);

        builder.HasIndex(x => x.OrcamentoId);

        builder.HasOne(x => x.Orcamento)
            .WithMany()
            .HasForeignKey(x => x.OrcamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}