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

        builder.Property(i => i.OrdemServicoId)
            .IsRequired();

        builder.Property(i => i.PecaServicoId)
            .IsRequired();

        builder.Ignore(i => i.Descricao);
        builder.Ignore(i => i.Valor);
        builder.Ignore(i => i.ValorTotal);
        builder.Ignore(i => i.Pecas);

        builder.HasOne(i => i.PecaServico)
            .WithMany()
            .HasForeignKey(i => i.PecaServicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OrdemServico>()
            .WithMany()
            .HasForeignKey(i => i.OrdemServicoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}