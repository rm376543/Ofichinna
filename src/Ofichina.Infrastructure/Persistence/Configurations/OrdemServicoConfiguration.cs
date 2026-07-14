using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class OrdemServicoConfiguration : IEntityTypeConfiguration<OrdemServico>
{
    public void Configure(EntityTypeBuilder<OrdemServico> builder)
    {
        builder.ToTable("OrdensServico");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PessoaId).IsRequired();
        builder.Property(x => x.VeiculoId).IsRequired();
        builder.Property(x => x.FuncionarioId).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.DataAbertura).IsRequired();
        builder.Property(x => x.DataFinalizacao);
        builder.Property(x => x.Observacao).HasMaxLength(500);

        builder.Navigation(x => x.Servicos)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(x => x.Pecas)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne<Pessoa>()
            .WithMany()
            .HasForeignKey(x => x.PessoaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Veiculo>()
            .WithMany()
            .HasForeignKey(x => x.VeiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Pessoa>()
            .WithMany()
            .HasForeignKey(x => x.FuncionarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Servicos)
            .WithOne()
            .HasForeignKey(x => x.OrdemServicoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Pecas)
            .WithOne()
            .HasForeignKey(x => x.OrdemServicoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}