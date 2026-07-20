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
        builder.Property(x => x.DiaDisponibilidadeId).IsRequired();
        builder.Property(x => x.HorarioConsultorId).IsRequired();
        builder.Property(x => x.ConsultorPessoaId).IsRequired();
        builder.Property(x => x.VeiculoId).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Descricao).HasMaxLength(1000);

        builder.HasOne(x => x.Cliente)
            .WithMany()
            .HasForeignKey(x => x.ClientePessoaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DiaDisponibilidade)
            .WithMany()
            .HasForeignKey(x => x.DiaDisponibilidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.HorarioConsultor)
            .WithMany()
            .HasForeignKey(x => x.HorarioConsultorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Consultor)
            .WithMany()
            .HasForeignKey(x => x.ConsultorPessoaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Veiculo)
            .WithMany()
            .HasForeignKey(x => x.VeiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.DiaDisponibilidadeId, x.HorarioConsultorId })
            .IsUnique();

        builder.HasIndex(x => x.ClientePessoaId);
        builder.HasIndex(x => x.ConsultorPessoaId);
        builder.HasIndex(x => x.DiaDisponibilidadeId);
        builder.HasIndex(x => x.HorarioConsultorId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.VeiculoId);
    }
}