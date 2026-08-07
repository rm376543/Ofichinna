using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public sealed class HorarioDisponibilidadeConfiguration : IEntityTypeConfiguration<HorarioDisponibilidade>
{
    public void Configure(EntityTypeBuilder<HorarioDisponibilidade> builder)
    {
        builder.ToTable("HorariosDisponibilidade");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("HorarioDisponibilidadeId");

        builder.Property(x => x.Hora)
            .HasColumnType("time")
            .IsRequired();

        builder.HasIndex(x => x.Hora).IsUnique();

        builder.Navigation(x => x.Consultores)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(x => x.Dias)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}