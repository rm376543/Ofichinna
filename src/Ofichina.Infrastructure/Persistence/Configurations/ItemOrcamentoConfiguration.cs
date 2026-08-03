using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class ItemOrcamentoConfiguration : IEntityTypeConfiguration<ItemOrcamento>
{
    public void Configure(EntityTypeBuilder<ItemOrcamento> builder)
    {
        builder.ToTable("ItensOrcamento");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.OrcamentoId).IsRequired();
        builder.Property(i => i.ServicoId).IsRequired();

        builder.Navigation(i => i.Pecas)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(i => i.Servico)
            .WithMany()
            .HasForeignKey(i => i.ServicoId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasMany(i => i.Pecas)
            .WithOne()
            .HasForeignKey(i => i.ItemOrcamentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Orcamento)
            .WithMany(x => x.Servicos)
            .HasForeignKey(i => i.OrcamentoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
