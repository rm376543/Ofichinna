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

        builder.Property(i => i.Descricao)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.Valor)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        // Propriedade calculada no domínio
        builder.Ignore(i => i.ValorTotal);

        builder.HasMany(x => x.Pecas)
            .WithOne()
            .HasForeignKey(i => i.ItemServicoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Pecas)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne<OrdemServico>()
            .WithMany()
            .HasForeignKey(i => i.OrdemServicoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Servico>()
            .WithMany()
            .HasForeignKey(i => i.ServicoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}