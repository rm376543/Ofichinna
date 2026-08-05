using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class ItemServicoConfiguration : IEntityTypeConfiguration<ItemServico>
{
    public void Configure(EntityTypeBuilder<ItemServico> builder)
    {
        builder.ToTable("ItensServico");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.OrcamentoId);

        builder.Property(i => i.OrdemServicoId);

        builder.Property(i => i.ServicoId)
            .IsRequired();

        builder.Property(i => i.PecaId)
            .IsRequired();

        builder.Property(i => i.Quantidade)
            .IsRequired();

        builder.HasOne(i => i.Servico)
            .WithMany()
            .HasForeignKey(i => i.ServicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Peca)
            .WithMany()
            .HasForeignKey(i => i.PecaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Orcamento)
            .WithMany(x => x.ItensServico)
            .HasForeignKey(i => i.OrcamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.OrdemServico)
            .WithMany(x => x.Servicos)
            .HasForeignKey(i => i.OrdemServicoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}