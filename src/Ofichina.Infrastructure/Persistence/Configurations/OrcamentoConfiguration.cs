using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class OrcamentoConfiguration : IEntityTypeConfiguration<Orcamento>
{
    public void Configure(EntityTypeBuilder<Orcamento> builder)
    {
        builder.ToTable("Orcamentos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PessoaId).IsRequired();
        builder.Property(x => x.VeiculoId).IsRequired();
        builder.Property(x => x.MecanicoDiagnosticoId).IsRequired();
        builder.Property(x => x.ResponsavelId).IsRequired();
        builder.Property(x => x.DataValidade).IsRequired();
        builder.Property(x => x.Desconto).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Observacoes).HasMaxLength(1000);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();

        builder.Navigation(x => x.Servicos)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne<Pessoa>()
            .WithMany()
            .HasForeignKey(x => x.PessoaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Veiculo>()
            .WithMany()
            .HasForeignKey(x => x.VeiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Pessoa>()
            .WithMany()
            .HasForeignKey(x => x.MecanicoDiagnosticoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Pessoa>()
            .WithMany()
            .HasForeignKey(x => x.ResponsavelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Checklist)
            .WithOne(x => x.Orcamento!)
            .HasForeignKey<Checklist>(x => x.OrcamentoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
