using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public sealed class DiaHorarioDisponibilidadeConfiguration : IEntityTypeConfiguration<DiaHorarioDisponibilidade>
{
    public void Configure(EntityTypeBuilder<DiaHorarioDisponibilidade> builder)
    {
        builder.ToTable("DiasHorariosDisponibilidade");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("DiaHorarioDisponibilidadeId");

        builder.Property(x => x.DiaDisponibilidadeId).IsRequired();
        builder.Property(x => x.HorarioDisponibilidadeId).IsRequired();

        builder.HasOne(x => x.DiaDisponibilidade)
            .WithMany(x => x.Horarios)
            .HasForeignKey(x => x.DiaDisponibilidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.HorarioDisponibilidade)
            .WithMany(x => x.Dias)
            .HasForeignKey(x => x.HorarioDisponibilidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.DiaDisponibilidadeId, x.HorarioDisponibilidadeId })
            .IsUnique();

        builder.HasIndex(x => x.HorarioDisponibilidadeId);
    }
}