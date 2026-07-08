using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class ExemploConfiguration : IEntityTypeConfiguration<Exemplo>
{
    public void Configure(EntityTypeBuilder<Exemplo> builder)
    {
        builder.ToTable("Exemplos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Descricao)
            .HasMaxLength(500);

        builder.Property(x => x.Ativo)
            .IsRequired();
    }
}