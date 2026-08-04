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

        builder.Property(x => x.VeiculoId).IsRequired();
        builder.Property(x => x.PessoaId).IsRequired();
        builder.Property(x => x.HodometroEntrada).IsRequired();
        builder.Property(x => x.ItensVerificados).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.Observacoes).HasMaxLength(1000);
        builder.Property(x => x.Finalizado).IsRequired();

        builder.HasOne(x => x.Veiculo)
            .WithMany()
            .HasForeignKey(x => x.VeiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Pessoa)
            .WithMany()
            .HasForeignKey(x => x.PessoaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
