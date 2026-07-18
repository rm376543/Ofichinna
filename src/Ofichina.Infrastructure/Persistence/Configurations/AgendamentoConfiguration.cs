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

        builder.Property(x => x.ClientePessoaId).IsRequired();
        builder.Property(x => x.ConsultorPessoaId).IsRequired();
        builder.Property(x => x.VeiculoId).IsRequired();
        builder.Property(x => x.DataAgendamento).HasColumnType("date").IsRequired();
        builder.Property(x => x.HorarioAgendamento).HasColumnType("time").IsRequired();
        builder.Property(x => x.Descricao).HasMaxLength(1000);

        builder.HasOne<Pessoa>()
            .WithMany()
            .HasForeignKey(x => x.ClientePessoaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Pessoa>()
            .WithMany()
            .HasForeignKey(x => x.ConsultorPessoaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ConsultorPessoaId, x.DataAgendamento, x.HorarioAgendamento })
            .IsUnique();

        builder.HasOne<Veiculo>()
            .WithMany()
            .HasForeignKey(x => x.VeiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ClientePessoaId);
        builder.HasIndex(x => x.ConsultorPessoaId);
        builder.HasIndex(x => x.DataAgendamento);
    }
}