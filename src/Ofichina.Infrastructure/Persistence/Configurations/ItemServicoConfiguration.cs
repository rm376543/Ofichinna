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

        builder.HasOne<OrdemServico>()
            .WithMany()
            .HasForeignKey(i => i.OrdemServicoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}