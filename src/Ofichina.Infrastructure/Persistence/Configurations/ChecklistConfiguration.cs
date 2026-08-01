using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class ChecklistConfiguration : IEntityTypeConfiguration<Checklist>
{
    public void Configure(EntityTypeBuilder<Checklist> builder)
    {
        builder.ToTable("Checklists");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrcamentoId).IsRequired();
        builder.Property(x => x.HodometroEntrada).IsRequired();
        builder.Property(x => x.ItensVerificados).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.Observacoes).HasMaxLength(1000);

        builder.HasOne(x => x.Orcamento)
            .WithOne(x => x.Checklist)
            .HasForeignKey<Checklist>(x => x.OrcamentoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
