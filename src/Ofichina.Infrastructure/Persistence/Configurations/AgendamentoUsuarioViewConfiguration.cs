using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração EF para a view vwAgendamentosUsuario.
/// </summary>
public sealed class AgendamentoUsuarioViewConfiguration : IEntityTypeConfiguration<AgendamentoUsuarioView>
{
    public void Configure(EntityTypeBuilder<AgendamentoUsuarioView> builder)
    {
        builder.HasNoKey();
        builder.ToView("vwAgendamentosUsuario");

        builder.Property(x => x.AgendamentosId).HasColumnName("AgendamentosId");
        builder.Property(x => x.PessoaId).HasColumnName("PessoaId");
        builder.Property(x => x.Nome).HasColumnName("Nome");
        builder.Property(x => x.Documento).HasColumnName("Documento");
        builder.Property(x => x.Telefone).HasColumnName("Telefone");
        builder.Property(x => x.Placa).HasColumnName("Placa");
        builder.Property(x => x.Marca).HasColumnName("Marca");
        builder.Property(x => x.Modelo).HasColumnName("Modelo");
        builder.Property(x => x.AnoFabricacao).HasColumnName("AnoFabricacao");
        builder.Property(x => x.Cor).HasColumnName("Cor");
        builder.Property(x => x.Hodometro).HasColumnName("Hodometro");
        builder.Property(x => x.Consultor).HasColumnName("Consultor");
        builder.Property(x => x.DtAgendamento).HasColumnName("DtAgendamento");
        builder.Property(x => x.HorarioAgendamento).HasColumnName("HorarioAgendamento");
        builder.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        builder.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
        builder.Property(x => x.DeletedAt).HasColumnName("DeletedAt");
    }
}
