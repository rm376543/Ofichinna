using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.ToTable("Agendamentos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PessoaId).IsRequired();
        builder.Property(x => x.VeiculoId).IsRequired();
        builder.Property(x => x.DataHoraAgendada).IsRequired();
        builder.Property(x => x.Motivo).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Observacoes).HasMaxLength(1000);
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.CanalAtendimento).IsRequired();

        builder.HasOne<Pessoa>()
            .WithMany()
            .HasForeignKey(x => x.PessoaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Veiculo>()
            .WithMany()
            .HasForeignKey(x => x.VeiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.PessoaId, x.DataHoraAgendada });
        builder.HasIndex(x => x.Status);
    }
}