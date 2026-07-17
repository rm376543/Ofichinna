using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class VeiculoConfiguration : IEntityTypeConfiguration<Veiculo>
{
    public void Configure(EntityTypeBuilder<Veiculo> builder)
    {
        builder.ToTable("Veiculos");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.PessoaId)
            .IsRequired();

        builder.Property(v => v.Placa)
            .HasConversion(
                placa => placa.Numero,
                valor => new Placa(valor))
            .HasMaxLength(7)
            .IsRequired();

        builder.Property(v => v.Marca)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(v => v.Modelo)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(v => v.AnoFabricacao)
            .IsRequired();

        builder.Property(v => v.Cor)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(v => v.Observacoes)
            .HasMaxLength(1000);

        builder.Property(v => v.Hodometro)
            .HasConversion(
                hodometro => hodometro.Valor,
                valor => new Hodometro(valor))
            .IsRequired();

        builder.Property(v => v.Ativo)
            .IsRequired();

        builder.HasOne(v => v.Pessoa)
            .WithMany(p => p.Veiculos)
            .HasForeignKey(v => v.PessoaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(v => v.Placa)
            .IsUnique();
    }
}