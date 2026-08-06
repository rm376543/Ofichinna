using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.Enums;
using Ofichina.Infrastructure.Persistence.Converters;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.ToTable("Agendamentos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClientePessoaId).IsRequired();
        builder.Property(x => x.VeiculoId).IsRequired();
        builder.Property(x => x.HorarioConsultorDisponibilidadeId).IsRequired();

        // Campos legados - mantidos temporariamente para compatibilidade com migration
        builder.Property(x => x.DiaDisponibilidadeId).IsRequired(false);
        builder.Property(x => x.HorarioConsultorId).IsRequired(false);
        builder.Property(x => x.ConsultorPessoaId).IsRequired(false);

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

        builder.HasOne(x => x.HorarioConsultorDisponibilidade)
            .WithMany()
            .HasForeignKey(x => x.HorarioConsultorDisponibilidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamentos legados - mantidos temporariamente
        builder.HasOne(x => x.DiaDisponibilidade)
            .WithMany()
            .HasForeignKey(x => x.DiaDisponibilidadeId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(x => x.HorarioConsultor)
            .WithMany()
            .HasForeignKey(x => x.HorarioConsultorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(x => x.Consultor)
            .WithMany()
            .HasForeignKey(x => x.ConsultorPessoaId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Índices
        builder.HasIndex(x => x.HorarioConsultorDisponibilidadeId);
        builder.HasIndex(x => x.ClientePessoaId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.VeiculoId);

        // Índices legados - podem ser removidos em migrations futuras
        builder.HasIndex(x => x.ConsultorPessoaId);
        builder.HasIndex(x => x.DiaDisponibilidadeId);
        builder.HasIndex(x => x.HorarioConsultorId);
    }
}