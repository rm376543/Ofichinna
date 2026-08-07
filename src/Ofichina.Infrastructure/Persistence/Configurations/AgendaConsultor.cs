using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração EF Core para AgendaConsultor.
/// </summary>
public sealed class AgendaConsultorConfiguration : IEntityTypeConfiguration<AgendaConsultor>
{
    public void Configure(EntityTypeBuilder<AgendaConsultor> builder)
    {
        builder.ToTable("AgendaConsultor");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("AgendamentoConsultorId");

        builder.Property(x => x.DiaDisponibilidadeId).IsRequired();
        builder.Property(x => x.HorarioDisponibilidadeId).IsRequired();
        builder.Property(x => x.ConsultorPessoaId).IsRequired();

        builder.HasOne(x => x.DiaDisponibilidade)
            .WithMany()
            .HasForeignKey(x => x.DiaDisponibilidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.HorarioDisponibilidade)
            .WithMany()
            .HasForeignKey(x => x.HorarioDisponibilidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Consultor)
            .WithMany()
            .HasForeignKey(x => x.ConsultorPessoaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índice único: garante que não haverá duplicação de slot (Dia + Horário + Consultor)
        builder.HasIndex(x => new { x.DiaDisponibilidadeId, x.HorarioDisponibilidadeId, x.ConsultorPessoaId })
            .IsUnique()
            .HasDatabaseName("IX_AgendaConsultor_DiaHorarioConsultor");

        // Índices individuais para queries
        builder.HasIndex(x => x.DiaDisponibilidadeId);
        builder.HasIndex(x => x.HorarioDisponibilidadeId);
        builder.HasIndex(x => x.ConsultorPessoaId);
    }
}
