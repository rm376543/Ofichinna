using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Enums;
using Ofichina.Infrastructure.Persistence.Converters;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class OrcamentoConfiguration : IEntityTypeConfiguration<Orcamento>
{
    public void Configure(EntityTypeBuilder<Orcamento> builder)
    {
        builder.ToTable("Orcamentos");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("OrcamentoId");

        builder.Property(x => x.PessoaId).IsRequired();
        builder.Property(x => x.VeiculoId).IsRequired();
        builder.Property(x => x.AgendamentoId).IsRequired();
        builder.Property(x => x.MecanicoId).IsRequired();
        builder.Property(x => x.ConsultorId).IsRequired();
        builder.Property(x => x.DataValidade).IsRequired();
        builder.Property(x => x.Desconto).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Observacoes).HasMaxLength(1000);
        builder.Property(x => x.Status)
            .HasConversion(new EnumParaTextoConverter<StatusOrcamento>())
            .HasMaxLength(40)
            .IsRequired();

        builder.Navigation(x => x.ItensServico)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne<Ofichina.Domain.Entities.Pessoa>()
            .WithMany()
            .HasForeignKey(x => x.PessoaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Ofichina.Domain.Entities.Veiculo>()
            .WithMany()
            .HasForeignKey(x => x.VeiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Agendamento)
            .WithMany()
            .HasForeignKey(x => x.AgendamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Ofichina.Domain.Entities.Pessoa>()
            .WithMany()
            .HasForeignKey(x => x.MecanicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Ofichina.Domain.Entities.Pessoa>()
            .WithMany()
            .HasForeignKey(x => x.ConsultorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
