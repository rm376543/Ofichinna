using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class ItemPecaConfiguration : IEntityTypeConfiguration<ItemPeca>
{
    public void Configure(EntityTypeBuilder<ItemPeca> builder)
    {
        builder.ToTable("ItensPeca");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.OrdemServicoId)
            .IsRequired();

        builder.Property(i => i.Descricao)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.Quantidade)
            .IsRequired();

        builder.Property(i => i.ValorUnitario)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(i => i.Utilizada)
            .IsRequired();

        builder.Property(i => i.DataUtilizacao);

        // Propriedade calculada no domínio
        builder.Ignore(i => i.ValorTotal);

        builder.HasOne<OrdemServico>()
            .WithMany()
            .HasForeignKey(i => i.OrdemServicoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}