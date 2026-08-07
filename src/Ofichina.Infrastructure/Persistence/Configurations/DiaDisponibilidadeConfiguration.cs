using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public sealed class DiaDisponibilidadeConfiguration : IEntityTypeConfiguration<DiaDisponibilidade>
{
    public void Configure(EntityTypeBuilder<DiaDisponibilidade> builder)
    {
        builder.ToTable("DiasDisponibilidade");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("DiaDisponibilidadeId");

        builder.Property(x => x.Data)
            .HasColumnType("date")
            .IsRequired();

        builder.HasIndex(x => x.Data).IsUnique();

        builder.Navigation(x => x.Horarios)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}