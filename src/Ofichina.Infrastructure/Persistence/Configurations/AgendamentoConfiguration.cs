using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Enums;
using Ofichina.Infrastructure.Persistence.Converters;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.ToTable("Agendamentos");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("AgendamentosId");

        builder.Property(x => x.ClientePessoaId).IsRequired();
        builder.Property(x => x.VeiculoId).IsRequired();
        builder.Property(x => x.AgendaConsultorId).IsRequired();

        builder.Property(x => x.Status)
            .HasConversion(new EnumParaTextoConverter<StatusAgendamento>())
            .HasMaxLength(40)
            .IsRequired();
        builder.Property(x => x.Descricao).HasMaxLength(1000);

        builder.HasOne(x => x.Cliente)
            .WithMany()
            .HasForeignKey(x => x.ClientePessoaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Veiculo)
            .WithMany()
            .HasForeignKey(x => x.VeiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AgendaConsultor)
            .WithMany()
            .HasForeignKey(x => x.AgendaConsultorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(x => x.AgendaConsultorId);
        builder.HasIndex(x => x.ClientePessoaId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.VeiculoId);
    }
}