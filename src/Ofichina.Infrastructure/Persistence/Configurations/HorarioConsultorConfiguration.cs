using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public sealed class HorarioConsultorConfiguration : IEntityTypeConfiguration<HorarioConsultor>
{
    public void Configure(EntityTypeBuilder<HorarioConsultor> builder)
    {
        builder.ToTable("HorariosConsultores");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.HorarioDisponibilidadeId).IsRequired();
        builder.Property(x => x.PessoaId).IsRequired();

        builder.HasOne(x => x.HorarioDisponibilidade)
            .WithMany(x => x.Consultores)
            .HasForeignKey(x => x.HorarioDisponibilidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Pessoa)
            .WithMany()
            .HasForeignKey(x => x.PessoaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.HorarioDisponibilidadeId, x.PessoaId })
            .IsUnique();

        builder.HasIndex(x => x.PessoaId);
    }
}