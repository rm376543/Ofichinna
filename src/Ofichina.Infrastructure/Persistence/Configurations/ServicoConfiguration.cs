using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração EF Core para a entidade Serviço.
/// </summary>
public class ServicoConfiguration : IEntityTypeConfiguration<Servico>
{
    public void Configure(EntityTypeBuilder<Servico> builder)
    {
        builder.ToTable("Servicos");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("ServicoId");

        builder.Property(s => s.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(s => s.Descricao)
            .HasMaxLength(500);

        builder.Property(s => s.Valor)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

    }
}