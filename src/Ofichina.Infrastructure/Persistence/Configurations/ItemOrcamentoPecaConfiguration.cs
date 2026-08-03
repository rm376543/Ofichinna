using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class ItemOrcamentoPecaConfiguration : IEntityTypeConfiguration<ItemOrcamentoPeca>
{
    public void Configure(EntityTypeBuilder<ItemOrcamentoPeca> builder)
    {
        builder.ToTable("ItensOrcamentoPecas");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ItemOrcamentoId).IsRequired();
        builder.Property(x => x.PecaId).IsRequired();
        builder.Property(x => x.Quantidade).IsRequired();

        builder.HasOne(x => x.Peca)
            .WithMany()
            .HasForeignKey(x => x.PecaId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
